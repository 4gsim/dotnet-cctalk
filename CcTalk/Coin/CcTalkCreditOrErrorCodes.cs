using System.Collections.Generic;

namespace CcTalk.Coin;

public class CcTalkCreditOrErrorCodes(byte counter, IEnumerable<ICcTalkCreditOrErrorCode> events)
{
    public byte Counter { get; } = counter;
    public IEnumerable<ICcTalkCreditOrErrorCode> Events { get; } = events;
}