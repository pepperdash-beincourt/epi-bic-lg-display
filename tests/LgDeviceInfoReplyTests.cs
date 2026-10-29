using Epi.Display.Lg;
using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugins.Lg.Display.Tests;

/// <summary>
/// Serial number and software version replies on the plain command set. The gather has already
/// taken off the trailing x when a reply arrives here.
/// </summary>
public class LgDeviceInfoReplyTests
{
    [Fact]
    public void Asks_the_configured_display()
    {
        LgDeviceInfoReply.SerialNumberQuery("01").Should().Be("fy 01 FF");
        LgDeviceInfoReply.SoftwareVersionQuery("01").Should().Be("fz 01 FF");
    }

    [Theory]
    [InlineData("y 01 OK405KCAB1234", "405KCAB1234")]
    [InlineData("z 01 OK03.20.50", "03.20.50")]
    [InlineData("z 01 OK03.20.50x", "03.20.50")]
    [InlineData("  y 1 OKSERIALOK99 \r", "SERIALOK99")]
    public void Reads_everything_after_OK(string reply, string expected)
    {
        LgDeviceInfoReply.Value(reply).Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("y 01 NG01")]
    [InlineData("y 01 OK")]
    public void An_NG_or_empty_reply_has_no_value(string? reply)
    {
        LgDeviceInfoReply.Value(reply!).Should().BeNull();
    }

    [Theory]
    [InlineData("60-75-6C-53-AF-93", "60:75:6c:53:af:93")]
    [InlineData("60:75:6c:53:af:93", "60:75:6c:53:af:93")]
    [InlineData("6075.6c53.af93", "60:75:6c:53:af:93")]
    [InlineData("not-a-mac", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void Formats_the_configured_mac(string? configured, string? expected)
    {
        LgDeviceInfoReply.FormatMac(configured!).Should().Be(expected);
    }
}
