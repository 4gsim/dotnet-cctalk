using System.Threading.Tasks;
using CcTalk.Hopper;

namespace CcTalk.Commands;

public class RequestHopperStatus(ICcTalkReceiver receiver) : ICcTalkCommand<CcTalkHopperStatus>
{
    public async Task<(CcTalkError?, CcTalkHopperStatus?)> ExecuteAsync(byte source, byte destination, int timeout)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 166,
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        var data = reply!.Value.Data;
        return (null, new CcTalkHopperStatus(data[0], data[1], data[2], data[3]));
    }
}