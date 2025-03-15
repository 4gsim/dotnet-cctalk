using System.Threading.Tasks;

namespace Dotnet.CcTalk.Commands;

public class ReadBufferedCreditOrErrorCodes(ICcTalkReceiver receiver) : ICcTalkCommand<byte[]>
{
    public async Task<(CcTalkError?, byte[]?)> ExecuteAsync()
    {
        var reply = new CcTalkDataBlock();
        var ret = await receiver.TryReceiveAsync(new CcTalkDataBlock()
        {
            Header = 229,
        }, ref reply);
        if (ret != null)
        {
            return (ret, null);
        }
        return (null, reply.Data);
    }
}