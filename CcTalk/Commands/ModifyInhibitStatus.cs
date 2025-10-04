using System.Threading.Tasks;

namespace CcTalk.Commands;

public class ModifyInhibitStatus(ICcTalkReceiver receiver, int status) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 231,
            Data = [(byte)(status & 0xFF), (byte)((status >> 8) & 0xFF)]
        }, timeout);
    }
}