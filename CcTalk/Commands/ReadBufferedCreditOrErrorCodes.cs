using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ReadBufferedCreditOrErrorCodes(ICcTalkReceiver receiver) : ICcTalkCommand<byte[]>
{
    public async Task<(CcTalkError?, byte[]?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock()
        {
            Source = source,
            Destination = destination,
            Header = 229,
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        return (null, reply!.Value.Data);
    }
}