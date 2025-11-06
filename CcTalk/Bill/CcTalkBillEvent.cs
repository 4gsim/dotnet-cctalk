namespace CcTalk.Bill;

public readonly struct CcTalkBillEvent(int index, byte position) : ICcTalkBillEvent
{
    public int Index { get; } = index;
    public CcTalkBillPosition Position { get; } = (CcTalkBillPosition)position;
}