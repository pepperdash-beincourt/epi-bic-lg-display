using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugins.Lg.Display.Tests;

public class LgIpControlCipherTests
{
    // Published example: keycode 12345678, an all-zero IV and the command "VOLUME_MUTE on".
    private const string ExampleKeycode = "12345678";
    private const string ExampleCommand = "VOLUME_MUTE on";
    private const string ExampleMessageHex = "d2b21ca0ad6486cb2056a8b815033508dfe77a7de05603a59ed5316ec552fac1";

    [Fact]
    public void Encrypts_The_Published_Example()
    {
        var cipher = new LgIpControlCipher(ExampleKeycode);

        var message = cipher.Encrypt(ExampleCommand, new byte[16]);

        Convert.ToHexString(message).ToLowerInvariant().Should().Be(ExampleMessageHex);
    }

    [Fact]
    public void Decrypts_The_Published_Example()
    {
        var cipher = new LgIpControlCipher(ExampleKeycode);

        var plain = cipher.Decrypt(Convert.FromHexString(ExampleMessageHex));

        plain.Should().StartWith(ExampleCommand + "\r");
    }

    [Theory]
    [InlineData("OK")]
    [InlineData("VOL:43")]
    [InlineData("APP:com.webos.app.hdmi2 Hot plug:Connected Signal:Yes HDCP:2.2 HDCP Status:Authenticated")]
    [InlineData("0123456789ABCDE")] // the terminator is the last byte of the first block
    public void Receives_A_Response_Split_Across_Reads(string response)
    {
        var display = new LgIpControlCipher("AB12CD34");
        var received = new List<string>();
        display.ResponseReceived += received.Add;

        var message = EncryptResponse(display, response);
        foreach (var chunk in message.Chunk(5))
            display.Receive(chunk);

        received.Should().Equal(response);
    }

    [Fact]
    public void Receives_Two_Responses_In_One_Read()
    {
        var display = new LgIpControlCipher("AB12CD34");
        var received = new List<string>();
        display.ResponseReceived += received.Add;

        display.Receive(EncryptResponse(display, "MUTE:on").Concat(EncryptResponse(display, "VOL:7")).ToArray());

        received.Should().Equal("MUTE:on", "VOL:7");
    }

    [Fact]
    public void Reports_Data_Encrypted_With_Another_Keycode()
    {
        var display = new LgIpControlCipher("AB12CD34");
        var undecryptable = 0;
        var received = new List<string>();
        display.ResponseReceived += received.Add;
        display.UndecryptableResponse += () => undecryptable++;

        var other = new LgIpControlCipher("ZZ99YY88");
        for (byte i = 0; i < 10; i++)
            display.Receive(other.Encrypt("CURRENT_APP", Enumerable.Repeat(i, 16).ToArray()));

        undecryptable.Should().BeGreaterThan(0);
        received.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("ab12cd34")]
    public void Rejects_An_Invalid_Keycode(string? keycode)
    {
        LgIpControlCipher.IsValidKeycode(keycode!).Should().BeFalse();
        var construct = () => new LgIpControlCipher(keycode!);
        construct.Should().Throw<ArgumentException>();
    }

    // The display encrypts responses the same way, but ends them with a line feed instead of a
    // carriage return. Build one by encrypting with a terminator the command path would reject.
    private static byte[] EncryptResponse(LgIpControlCipher cipher, string response)
    {
        var withLineFeed = cipher.Encrypt(response + "\n", RandomIv());
        return withLineFeed;
    }

    private static byte[] RandomIv()
    {
        var iv = new byte[16];
        Random.Shared.NextBytes(iv);
        return iv;
    }
}
