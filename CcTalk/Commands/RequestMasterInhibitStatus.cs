using System.Threading.Tasks;

namespace CcTalk.Commands;

public class RequestMasterInhibitStatus(ICcTalkReceiver receiver) : ICcTalkCommand<bool>
{
    public async Task<(CcTalkError?, bool)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 227
        }, timeout);
        if (err != null)
        {
            return (err, false);
        }

        return (null, reply!.Value.Data[0] == 0);
    }
}