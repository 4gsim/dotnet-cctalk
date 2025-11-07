namespace CcTalk.Coin;

public class CcTalkCredit(byte index) : ICcTalkCreditOrErrorCode
{
    public byte Index { get; } = index;
    
    public override string ToString()
    {
        return $"Coin Index: {Index}";
    }
}