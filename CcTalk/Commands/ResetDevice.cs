using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ResetDevice(ICcTalkReceiver receiver) : ICcTalkCommand<object>
{
    public int WaitTime { get; set; }

    public async Task<(CcTalkError?, object?)> ExecuteAsync()
    {
        var (err, _) = await receiver.ReceiveAsync(new CcTalkDataBlock()
        {
            Header = 1
        });
        if (err != null)
        {
            return (err, null);
        }
        return (null, ICcTalkCommand<object?>.Success);
    }
}