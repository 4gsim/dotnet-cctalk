using System.Threading.Tasks;

namespace CcTalk.Commands;

public class SimplePoll(ICcTalkReceiver receiver) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync()
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock()
        {
            Header = 254,
        });
    }
}