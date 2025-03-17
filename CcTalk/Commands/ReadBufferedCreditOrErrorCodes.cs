using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ReadBufferedCreditOrErrorCodes(ICcTalkReceiver receiver) : ICcTalkCommand<byte[]>
{
    public async Task<(CcTalkError?, byte[]?)> ExecuteAsync()
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock()
        {
            Header = 229,
        });
        if (err != null)
        {
            return (err, null);
        }
        return (null, reply!.Value.Data);
    }
}