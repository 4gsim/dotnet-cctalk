namespace CcTalk.Coin;

public readonly struct CcTalkErrorCode(byte error) : ICcTalkCreditOrErrorCode
{
    public CcTalkCoinAcceptorError Error { get; } = (CcTalkCoinAcceptorError)error;
}