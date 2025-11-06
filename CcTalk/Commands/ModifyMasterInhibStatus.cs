using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ModifyMasterInhibitStatus(ICcTalkReceiver receiver, bool status) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 228,
            Data = [status ? (byte)0 : (byte)1]
        }, timeout);
    }
}