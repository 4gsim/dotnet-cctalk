using System.Linq;
using System.Threading.Tasks;
using CcTalk.Hopper;

namespace CcTalk.Commands;

public class TestHopper(ICcTalkReceiver receiver) : ICcTalkCommand<CcTalkHopperFlag>
{
    public async Task<(CcTalkError?, CcTalkHopperFlag)> ExecuteAsync(byte source, byte destination, int timeout)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 160
        }, timeout);
        if (err != null)
        {
            return (err, CcTalkHopperFlag.Nothing);
        }

        var value = reply!.Value.Data.Select((b, i) => b << i * 8).Sum();
        
        return (null, (CcTalkHopperFlag)value);
    }
}