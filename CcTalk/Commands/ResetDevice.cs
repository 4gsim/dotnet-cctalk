using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ResetDevice(ICcTalkReceiver receiver) : ICcTalkCommand<object>
{
    public int WaitTime { get; set; }

    public Task<(CcTalkError?, object?)> ExecuteAsync()
    {
        return ICcTalkCommand<object>.ExecuteWithoutResponseAsync(receiver, 1);
    }
}