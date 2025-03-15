using System.Threading.Tasks;

namespace Dotnet.CcTalk.Commands;

public class RequestCoinId(ICcTalkReceiver receiver, int coinId = -1) : ICcTalkCommand<string>
{
    public async Task<(CcTalkError?, string?)> ExecuteAsync()
    {
        var reply = new CcTalkDataBlock();
        var data = coinId > -1 ? new byte[] {(byte)(coinId + 1)} : [];
        var ret = await receiver.TryReceiveAsync(new CcTalkDataBlock()
        {
            Header = 184,
            Data = data
        }, ref reply);
        if (ret != null)
        {
            return (ret, null);
        }
        var text = "";
        for (var i = 0; i < reply.Data.Length; i++)
        {
            text += (char)reply.Data[i];
        }
        return (null, text);
    }
}