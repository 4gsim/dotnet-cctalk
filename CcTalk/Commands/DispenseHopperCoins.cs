using System;
using System.Threading.Tasks;

namespace CcTalk.Commands;

public class DispenseHopperCoins(ICcTalkReceiver receiver, byte[] securityCode, byte count) : ICcTalkCommand<byte?>
{
    public async Task<(CcTalkError?, byte?)> ExecuteAsync(byte source, byte destination, int timeout)
    {
        var input = new byte[securityCode.Length + 1];
        Array.Copy(securityCode, input, securityCode.Length);
        input[^1] = count;
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 154,
            Data = input
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        var output = reply!.Value.Data;

        if (output.Length > 0)
        {
            return (null, output[0]);
        }

        return (null, null);
    }
}