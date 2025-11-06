using System.Threading.Tasks;
using CcTalk.Bill;

namespace CcTalk.Commands;

public class RouteBill(ICcTalkReceiver receiver, CcTalkBillRouteCode code) : ICcTalkCommand<CcTalkBillErrorCode?>
{
    public async Task<(CcTalkError?, CcTalkBillErrorCode?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 154,
            Data = [(byte)code]
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        var data = reply!.Value.Data;

        if (data.Length > 0)
        {
            return (null, (CcTalkBillErrorCode)data[0]);
        }

        return (null, null);
    }
}