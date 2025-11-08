using System.Threading.Tasks;

namespace CcTalk.Commands;

public class PumpRng(ICcTalkReceiver receiver, byte[] seed) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 161,
            Data = seed
        }, timeout);
    }
}