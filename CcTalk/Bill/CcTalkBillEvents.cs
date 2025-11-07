using System.Collections.Generic;

namespace CcTalk.Bill;

public class CcTalkBillEvents(byte counter, IEnumerable<ICcTalkBillEvent> events)
{
    public byte Counter { get; } = counter;
    public IEnumerable<ICcTalkBillEvent> Events { get; } = events;
}