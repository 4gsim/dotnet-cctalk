using System.Threading.Tasks;

namespace Dotnet.CcTalk.Commands;

public class ModifyInhibitStatus(ICcTalkReceiver receiver, int status) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync()
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock()
        {
            Header = 231,
            Data = [(byte)(status & 0xFF), (byte)((status >> 8) & 0xFF)]
        });
    }
}