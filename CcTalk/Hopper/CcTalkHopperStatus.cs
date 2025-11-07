namespace CcTalk.Hopper;

public class CcTalkHopperStatus(byte eventCounter, byte coinsRemaining, byte coinsPaid, byte coinsUnpaid)
{
    public byte EventCounter { get; } = eventCounter;
    public byte CoinsRemaining { get; } = coinsRemaining;
    public byte CoinsPaid { get; } = coinsPaid;
    public byte CoinsUnpaid { get; } = coinsUnpaid;
}