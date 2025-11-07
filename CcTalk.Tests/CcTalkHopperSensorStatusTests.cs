using CcTalk.Hopper;
using NUnit.Framework;

namespace CcTalk.Tests;

public class CcTalkHopperSensorStatusTests
{
    [TestCase((byte)0x00, CcTalkHopperLevelSensorStatus.Disabled, CcTalkHopperLevelSensorStatus.Disabled)]
    [TestCase((byte)0x20, CcTalkHopperLevelSensorStatus.Disabled, CcTalkHopperLevelSensorStatus.Lower)]
    [TestCase((byte)0x22, CcTalkHopperLevelSensorStatus.Disabled, CcTalkHopperLevelSensorStatus.HigherOrEqual)]
    [TestCase((byte)0x10, CcTalkHopperLevelSensorStatus.HigherOrEqual, CcTalkHopperLevelSensorStatus.Disabled)]
    [TestCase((byte)0x30, CcTalkHopperLevelSensorStatus.HigherOrEqual, CcTalkHopperLevelSensorStatus.Lower)]
    [TestCase((byte)0x32, CcTalkHopperLevelSensorStatus.HigherOrEqual, CcTalkHopperLevelSensorStatus.HigherOrEqual)]
    [TestCase((byte)0x11, CcTalkHopperLevelSensorStatus.Lower, CcTalkHopperLevelSensorStatus.Disabled)]
    [TestCase((byte)0x31, CcTalkHopperLevelSensorStatus.Lower, CcTalkHopperLevelSensorStatus.Lower)]
    [TestCase((byte)0x33, CcTalkHopperLevelSensorStatus.Lower, CcTalkHopperLevelSensorStatus.HigherOrEqual)]
    public void Parse(byte input, CcTalkHopperLevelSensorStatus expectedLow, CcTalkHopperLevelSensorStatus expectedHigh)
    {
        var actualReturn = new CcTalkHopperSensorStatus(input);
        Assert.That(actualReturn.Low, Is.EqualTo(expectedLow));
        Assert.That(actualReturn.High, Is.EqualTo(expectedHigh));
    }
}