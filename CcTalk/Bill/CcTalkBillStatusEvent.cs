namespace CcTalk.Bill;

public readonly struct CcTalkBillStatusEvent(byte code) : ICcTalkBillEvent
{
    public CcTalkBillStatusEventCode Code { get; } = (CcTalkBillStatusEventCode)code;
}