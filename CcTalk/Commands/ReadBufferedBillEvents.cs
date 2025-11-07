using System.Collections.Generic;
using System.Threading.Tasks;
using CcTalk.Bill;

namespace CcTalk.Commands;

public class ReadBufferedBillEvents(ICcTalkReceiver receiver) : ICcTalkCommand<CcTalkBillEvents>
{
    private static IEnumerable<ICcTalkBillEvent> GetEvents(byte[] data)
    {
        for (var i = 1; i < data.Length; i += 2)
        {
            var data1 = data[i];
            var data2 = data[i + 1];
            if (data1 == 0x00)
            {
                yield return new CcTalkBillErrorEvent(data2);
            }
            else
            {
                yield return new CcTalkBillEvent((byte)(data1 - 1 & 15), data2);
            }
        }
    }

    public async Task<(CcTalkError?, CcTalkBillEvents?)> ExecuteAsync(
        byte source = 1, byte destination = 0, int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 159,
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        var data = reply!.Value.Data;
        var counter = data[0];

        return (null, new CcTalkBillEvents(counter, GetEvents(data)));
    }
}