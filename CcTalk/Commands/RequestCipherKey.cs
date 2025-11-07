using System.Threading.Tasks;

namespace CcTalk.Commands;

public class RequestCipherKey(ICcTalkReceiver receiver) : ICcTalkCommand<byte[]>
{
    public async Task<(CcTalkError?, byte[]?)> ExecuteAsync(byte source, byte destination, int timeout)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 160
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }
        return (null, reply!.Value.Data);
    }
}