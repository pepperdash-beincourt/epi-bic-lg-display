using System;
using System.Text.RegularExpressions;

namespace Epi.Display.Lg
{
    /// <summary>
    /// The serial number (<c>fy</c>) and software version (<c>fz</c>) queries and their replies, for
    /// displays controlled with the plain command set.
    /// </summary>
    /// <remarks>
    /// A reply reads <c>{command} {id} OK{value}x</c> - <c>y 01 OK405KCAB1234x</c>,
    /// <c>z 01 OK03.20.50x</c> - with the trailing <c>x</c> already taken off by the gather. The value
    /// is everything after OK, not a split on spaces or a removal of every "OK", either of which can
    /// cut into a serial number.
    ///
    /// Free of Crestron types so it can be tested on its own.
    /// </remarks>
    public static class LgDeviceInfoReply
    {
        /// <summary>The serial number query for the display with this set ID.</summary>
        public static string SerialNumberQuery(string id)
        {
            return string.Format("fy {0} FF", id);
        }

        /// <summary>The software version query for the display with this set ID.</summary>
        public static string SoftwareVersionQuery(string id)
        {
            return string.Format("fz {0} FF", id);
        }

        /// <summary>The value of an OK reply, or null for an NG reply or a reply without one.</summary>
        public static string Value(string reply)
        {
            if (string.IsNullOrEmpty(reply))
            {
                return null;
            }

            var text = reply.Trim();
            var ok = text.IndexOf(" OK", StringComparison.Ordinal);
            if (ok < 0)
            {
                return null;
            }

            var value = text.Substring(ok + 3).Trim();
            if (value.EndsWith("x", StringComparison.Ordinal))
            {
                value = value.Substring(0, value.Length - 1).Trim();
            }

            return value.Length == 0 ? null : value;
        }

        /// <summary>
        /// A configured MAC address in any common form (60-75-6C-53-AF-93, 60:75:6c:53:af:93,
        /// 6075.6c53.af93) as colon-separated lower case, or null when it is not a MAC address.
        /// </summary>
        public static string FormatMac(string configured)
        {
            if (string.IsNullOrWhiteSpace(configured))
            {
                return null;
            }

            var hex = Regex.Replace(configured.Trim(), @"[-:.]", string.Empty).ToLowerInvariant();
            if (!Regex.IsMatch(hex, "^[0-9a-f]{12}$"))
            {
                return null;
            }

            return string.Join(":", new[]
            {
                hex.Substring(0, 2), hex.Substring(2, 2), hex.Substring(4, 2),
                hex.Substring(6, 2), hex.Substring(8, 2), hex.Substring(10, 2),
            });
        }
    }
}
