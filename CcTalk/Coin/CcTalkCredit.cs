namespace CcTalk.Coin;

public class CcTalkCredit(byte index) : ICcTalkCreditOrErrorCode
{
    public byte Index { get; }
}