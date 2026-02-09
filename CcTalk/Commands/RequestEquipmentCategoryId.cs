using System.Threading.Tasks;

namespace CcTalk.Commands;

public class RequestEquipmentCategoryId(ICcTalkReceiver receiver) : ICcTalkCommand<string>
{
    public async Task<(CcTalkError?, string?)> ExecuteAsync(byte source = 1, byte destination = 0,
        int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 245
        }, timeout);
        if (err != null)
        {
            return (err, null);
        }

        var text = "";
        for (var i = 0; i < reply!.Value.Data.Length; i++)
        {
            text += (char)reply.Value.Data[i];
        }

        return (null, text);
    }
}