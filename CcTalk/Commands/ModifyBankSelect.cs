using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ModifyBankSelect(ICcTalkReceiver receiver) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync(int timeout = 1000)
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock()
        {
            Header = 179,
            Data = [1]
        }, timeout);
    }
}