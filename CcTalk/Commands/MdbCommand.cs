using System;
using System.Threading.Tasks;

namespace CcTalk.Commands;

public class MdbCommand(ICcTalkReceiver receiver, byte[] input) : ICcTalkCommand<Memory<byte>>
{
    public async Task<(CcTalkError?, Memory<byte>)> ExecuteAsync(byte source, byte destination, int timeout)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 122,
            Data = input
        }, timeout);

        if (err != null)
        {
            return (err, null);
        }

        if ((reply!.Value.Data[0] & 0x01) == 0x00)
        {
            return (CcTalkError.FromMessage("Receive Error"), null);
        }

        return (null, new ArraySegment<byte>(reply.Value.Data, 1, reply.Value.Data.Length - 1));
    }
}