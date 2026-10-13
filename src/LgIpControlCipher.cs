using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PepperDash.Essentials.Plugins.Lg.Display
{
    /// <summary>
    /// Encrypts commands for, and decrypts responses from, the keycode-secured network IP control
    /// interface of LG webOS displays (TCP 9761). The display shows an eight-character keycode under
    /// IP Control Setup; an AES-128 key is derived from it with PBKDF2-HMAC-SHA256.
    ///
    /// Each message on the wire is a 16-byte AES-ECB-encrypted IV followed by the AES-CBC-encrypted,
    /// padded text. Commands end with a carriage return; responses end with a line feed.
    ///
    /// This class has no Crestron dependencies so it can be unit tested on a desktop runtime.
    /// </summary>
    public class LgIpControlCipher
    {
        public const int BlockSize = 16;
        public const int KeycodeLength = 8;

        private const int KeyIterations = 16384;
        private const int KeyLength = 16;
        private const char CommandTerminator = '\r';
        private const char ResponseTerminator = '\n';

        // Responses longer than this without a terminator mean the keycode does not match the display.
        private const int MaxResponseBlocks = 16;

        private static readonly byte[] KeySalt =
        {
            0x63, 0x61, 0xb8, 0x0e, 0x9b, 0xdc, 0xa6, 0x63, 0x8d, 0x07, 0x20, 0xf2, 0xcc, 0x56, 0x8f, 0xb9
        };

        private static readonly Regex KeycodeFormat = new Regex("^[A-Z0-9]{8}$");

        private readonly byte[] key;
        private readonly List<byte> receiveBuffer = new List<byte>();
        private readonly object receiveLock = new object();

        /// <summary>
        /// Fires with the plain text of each complete response, without its line terminator.
        /// </summary>
        public event Action<string> ResponseReceived;

        /// <summary>
        /// Fires when buffered data cannot be decrypted into a terminated response, which almost always
        /// means the configured keycode does not match the one the display generated.
        /// </summary>
        public event Action UndecryptableResponse;

        public LgIpControlCipher(string keycode)
        {
            if (!IsValidKeycode(keycode))
                throw new ArgumentException("The keycode must be eight upper-case letters or digits.", nameof(keycode));

            key = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(keycode), KeySalt, KeyIterations,
                HashAlgorithmName.SHA256, KeyLength);
        }

        public static bool IsValidKeycode(string keycode)
        {
            return keycode != null && KeycodeFormat.IsMatch(keycode);
        }

        /// <summary>
        /// Encrypts a command with a random IV, ready to send to the display.
        /// </summary>
        public byte[] Encrypt(string command)
        {
            var iv = new byte[BlockSize];
            RandomNumberGenerator.Fill(iv);
            return Encrypt(command, iv);
        }

        /// <summary>
        /// Encrypts a command with the given IV. Exposed for testing against known vectors.
        /// </summary>
        public byte[] Encrypt(string command, byte[] iv)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (iv == null || iv.Length != BlockSize) throw new ArgumentException("The IV must be 16 bytes.", nameof(iv));
            if (command.IndexOf(CommandTerminator) >= 0) throw new ArgumentException("The command must not contain a carriage return.", nameof(command));

            var plain = Pad(Encoding.UTF8.GetBytes(command + CommandTerminator));

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                var encryptedIv = aes.EncryptEcb(iv, PaddingMode.None);
                var encryptedData = aes.EncryptCbc(plain, iv, PaddingMode.None);

                var message = new byte[encryptedIv.Length + encryptedData.Length];
                Buffer.BlockCopy(encryptedIv, 0, message, 0, encryptedIv.Length);
                Buffer.BlockCopy(encryptedData, 0, message, encryptedIv.Length, encryptedData.Length);
                return message;
            }
        }

        /// <summary>
        /// Decrypts one complete message (encrypted IV plus data blocks) and returns the raw plain text,
        /// including any terminator and padding.
        /// </summary>
        public string Decrypt(byte[] message)
        {
            if (message == null || message.Length < BlockSize * 2 || message.Length % BlockSize != 0)
                throw new ArgumentException("The message must be at least two whole 16-byte blocks.", nameof(message));

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                var iv = aes.DecryptEcb(new ReadOnlySpan<byte>(message, 0, BlockSize), PaddingMode.None);
                var plain = aes.DecryptCbc(new ReadOnlySpan<byte>(message, BlockSize, message.Length - BlockSize), iv, PaddingMode.None);
                return Encoding.UTF8.GetString(plain);
            }
        }

        /// <summary>
        /// Feeds bytes received from the display. Raises <see cref="ResponseReceived"/> for each complete
        /// response; partial responses are held until the rest arrives.
        /// </summary>
        public void Receive(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;

            var responses = new List<string>();
            var undecryptable = false;

            lock (receiveLock)
            {
                receiveBuffer.AddRange(bytes);

                while (true)
                {
                    var response = TryTakeResponse(out var consumed);
                    if (response != null)
                    {
                        receiveBuffer.RemoveRange(0, consumed);

                        // Text decrypted with the wrong key is random bytes; a real response is printable.
                        if (IsPrintable(response))
                        {
                            responses.Add(response);
                            continue;
                        }

                        receiveBuffer.Clear();
                        undecryptable = true;
                        break;
                    }

                    if (receiveBuffer.Count >= BlockSize * (MaxResponseBlocks + 1))
                    {
                        receiveBuffer.Clear();
                        undecryptable = true;
                    }
                    break;
                }
            }

            foreach (var response in responses)
                ResponseReceived?.Invoke(response);

            if (undecryptable)
                UndecryptableResponse?.Invoke();
        }

        /// <summary>
        /// Discards any partially received response, e.g. after the connection drops.
        /// </summary>
        public void Reset()
        {
            lock (receiveLock)
            {
                receiveBuffer.Clear();
            }
        }

        // Decrypts whole blocks after the encrypted IV until one contains the response terminator.
        private string TryTakeResponse(out int consumed)
        {
            consumed = 0;
            if (receiveBuffer.Count < BlockSize * 2) return null;

            var available = receiveBuffer.ToArray();

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                var previous = aes.DecryptEcb(new ReadOnlySpan<byte>(available, 0, BlockSize), PaddingMode.None);
                var text = new StringBuilder();

                for (var offset = BlockSize; offset + BlockSize <= available.Length; offset += BlockSize)
                {
                    var cipherBlock = new byte[BlockSize];
                    Buffer.BlockCopy(available, offset, cipherBlock, 0, BlockSize);

                    var plainBlock = aes.DecryptCbc(cipherBlock, previous, PaddingMode.None);
                    previous = cipherBlock;

                    var blockText = Encoding.UTF8.GetString(plainBlock);
                    var terminator = blockText.IndexOf(ResponseTerminator);
                    if (terminator >= 0)
                    {
                        text.Append(blockText, 0, terminator);
                        consumed = offset + BlockSize;
                        return text.ToString().TrimEnd('\r');
                    }

                    text.Append(blockText);
                }
            }

            return null;
        }

        private static bool IsPrintable(string text)
        {
            foreach (var c in text)
            {
                if (c < 0x20 || c > 0x7e) return false;
            }
            return true;
        }

        // LG pads to the block size with bytes whose value is the pad length. A message that is already
        // a whole number of blocks first gets a space, so there is always at least one pad byte.
        private static byte[] Pad(byte[] data)
        {
            var length = data.Length;
            if (length % BlockSize == 0) length++;

            var padding = BlockSize - (length % BlockSize);
            if (padding == BlockSize) padding = 0;

            var padded = new byte[length + padding];
            Buffer.BlockCopy(data, 0, padded, 0, data.Length);
            if (length > data.Length) padded[data.Length] = (byte)' ';
            for (var i = length; i < padded.Length; i++)
                padded[i] = (byte)padding;

            return padded;
        }
    }
}
