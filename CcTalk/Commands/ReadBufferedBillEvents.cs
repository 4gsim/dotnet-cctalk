using System.Collections.Generic;
using System.Threading.Tasks;
using CcTalk.Bill;

namespace CcTalk.Commands;

public class ReadBufferedBillEvents(ICcTalkReceiver receiver) : ICcTalkCommand<(byte counter, List<ICcTalkBillEvent>)?>
{
    public async Task<(CcTalkError?, (byte counter, List<ICcTalkBillEvent>)?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
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
        var events = new List<ICcTalkBillEvent>();
        for (var i = 1; i < data.Length; i += 2)
        {
            var data1 = data[i];
            var data2 = data[i + 1];
            if (data1 == 0x00)
            {
                events.Add(new CcTalkBillErrorEvent(data2));
            }
            else
            {
                events.Add(new CcTalkBillEvent(data1, data2));
            }
        }

        return (null, (counter, events));
    }
}