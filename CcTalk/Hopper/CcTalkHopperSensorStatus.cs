namespace CcTalk.Hopper;

public class CcTalkHopperSensorStatus
{
    public CcTalkHopperLevelSensorStatus Low { get; } 
    public CcTalkHopperLevelSensorStatus High { get; }

    public CcTalkHopperSensorStatus(byte value)
    {
        if ((value & 0x10) == 0x00)
        {
            Low = CcTalkHopperLevelSensorStatus.Disabled;
        }
        else if ((value & 0x01) == 0x00)
        {
            Low = CcTalkHopperLevelSensorStatus.HigherOrEqual;
        }
        else
        {
            Low = CcTalkHopperLevelSensorStatus.Lower;
        }

        if ((value & 0x20) == 0x00)
        {
            High = CcTalkHopperLevelSensorStatus.Disabled;
        }
        else if ((value & 0x02) == 0x00)
        {
            High = CcTalkHopperLevelSensorStatus.Lower;
        }
        else
        {
            High = CcTalkHopperLevelSensorStatus.HigherOrEqual;
        }
    }
}