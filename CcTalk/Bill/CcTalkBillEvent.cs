namespace CcTalk.Bill;

public readonly struct CcTalkBillEvent(byte index, byte position) : ICcTalkBillEvent
{
    public byte Index { get; } = index;
    public CcTalkBillPosition Position { get; } = (CcTalkBillPosition)position;
}