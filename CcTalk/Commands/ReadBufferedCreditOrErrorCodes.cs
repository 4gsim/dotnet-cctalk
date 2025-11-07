using System.Collections.Generic;
using System.Threading.Tasks;
using CcTalk.Coin;

namespace CcTalk.Commands;

public class ReadBufferedCreditOrErrorCodes(ICcTalkReceiver receiver)
    : ICcTalkCommand<(byte Counter, IEnumerable<ICcTalkCreditOrErrorCode> Events)?>
{
    private static IEnumerable<ICcTalkCreditOrErrorCode> GetEvents(byte[] data)
    {
        for (var i = 1; i < data.Length; i += 2)
        {
            var data1 = data[i];
            var data2 = data[i + 1];
            if (data1 == 0x00)
            {
                yield return new CcTalkErrorCode(data2);
            }
            else
            {
                yield return new CcTalkCredit((byte)(data1 - 1 & 15));
            }
        }
    }

    public async Task<(CcTalkError?, (byte Counter, IEnumerable<ICcTalkCreditOrErrorCode> Events)?)> ExecuteAsync(
        byte source = 1, byte destination = 0, int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 229,
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        var data = reply!.Value.Data;
        var counter = data[0];

        return (null, (counter, GetEvents(data)));
    }
}