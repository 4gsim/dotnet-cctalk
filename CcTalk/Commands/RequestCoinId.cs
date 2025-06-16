using System.Threading.Tasks;
using CcTalk.Coin;

namespace CcTalk.Commands;

public class RequestCoinId(ICcTalkReceiver receiver, int coinId = -1) : ICcTalkCommand<CcTalkCoin>
{
    public async Task<(CcTalkError?, CcTalkCoin)> ExecuteAsync(int timeout = 1000)
    {
        var data = coinId > -1 ? new byte[] {(byte)(coinId + 1)} : [];
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock()
        {
            Header = 184,
            Data = data
        }, timeout);
        if (err != null)
        {
            return (err, new CcTalkCoin());
        }
        var text = "";
        for (var i = 0; i < reply!.Value.Data.Length; i++)
        {
            text += (char)reply!.Value.Data[i];
        }
        if (!CcTalkCoinValueParser.TryParse(text, out CcTalkCoin coin))
        {
            return (CcTalkError.FromMessage("Parsed invalid coin value"), coin);
        }
        return (null, coin);
    }
}