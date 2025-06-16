using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ReadBufferedCreditOrErrorCodes(ICcTalkReceiver receiver) : ICcTalkCommand<byte[]>
{
    public async Task<(CcTalkError?, byte[]?)> ExecuteAsync(int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock()
        {
            Header = 229,
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }
        return (null, reply!.Value.Data);
    }
}