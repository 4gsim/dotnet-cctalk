using System.Threading.Tasks;

namespace Dotnet.CcTalk.Commands;

public class ModifyBankSelect(ICcTalkReceiver receiver) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync()
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock()
        {
            Header = 179,
            Data = [1]
        });
    }
}