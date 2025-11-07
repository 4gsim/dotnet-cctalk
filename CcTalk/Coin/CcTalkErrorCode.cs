namespace CcTalk.Coin;

public readonly struct CcTalkErrorCode(byte error) : ICcTalkCreditOrErrorCode
{
    public CcTalkCoinAcceptorError Error { get; } = (CcTalkCoinAcceptorError)error;
    
    public override string ToString()
    {
        return $"Coin Error: {Error}";
    }
}