using System.Threading.Tasks;
using CcTalk.Bill;

namespace CcTalk.Commands;

public class RequestBillId(ICcTalkReceiver receiver, int billId = -1) : ICcTalkCommand<CcTalkBill>
{
    public async Task<(CcTalkError?, CcTalkBill)> ExecuteAsync(byte source = 1, byte destination = 0,
        int timeout = 1000)
    {
        var data = billId > -1 ? new[] { (byte)(billId + 1) } : [];
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 157,
            Data = data
        }, timeout);
        if (err != null)
        {
            return (err, new CcTalkBill());
        }

        var text = "";
        for (var i = 0; i < reply!.Value.Data.Length; i++)
        {
            text += (char)reply.Value.Data[i];
        }

        if (!CcTalkBillValueParser.TryParse(text, out var bill))
        {
            return (CcTalkError.FromMessage("Parsed invalid bill value"), bill);
        }

        return (null, bill);
    }
}