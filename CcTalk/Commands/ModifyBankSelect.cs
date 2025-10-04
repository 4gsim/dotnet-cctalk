using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ModifyBankSelect(ICcTalkReceiver receiver) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 179,
            Data = [1]
        }, timeout);
    }
}