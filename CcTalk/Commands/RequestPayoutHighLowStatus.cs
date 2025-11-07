using System;
using System.Threading.Tasks;
using CcTalk.Hopper;

namespace CcTalk.Commands;

public class RequestPayoutHighLowStatus(ICcTalkReceiver receiver, byte? hopperNumber = null)
    : ICcTalkCommand<CcTalkHopperSensorStatus>
{
    public async Task<(CcTalkError?, CcTalkHopperSensorStatus?)> ExecuteAsync(byte source, byte destination,
        int timeout)
    {
        var data = hopperNumber != null ? new[] { (byte)hopperNumber } : Array.Empty<byte>();
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 217,
            Data = data
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        return (null, new CcTalkHopperSensorStatus(reply!.Value.Data[0]));
    }
}