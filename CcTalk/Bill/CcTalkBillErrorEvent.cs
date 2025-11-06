namespace CcTalk.Bill;

public readonly struct CcTalkBillErrorEvent(byte code) : ICcTalkBillEvent
{
    public CcTalkBillErrorCode Code { get; } = (CcTalkBillErrorCode)code;
}