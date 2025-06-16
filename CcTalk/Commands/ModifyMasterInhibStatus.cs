using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ModifyMasterInhibitStatus(ICcTalkReceiver receiver, bool status) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync(int timeout = 1000)
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock()
        {
            Header = 228,
            Data = [status ? (byte)1 : (byte)0]
        }, timeout);
    }
}