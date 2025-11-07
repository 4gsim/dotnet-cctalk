using System.Threading.Tasks;

namespace CcTalk.Commands;

public class EnableHopper(ICcTalkReceiver receiver, bool enabled) : ICcTalkCommand<object>
{
    public Task<(CcTalkError?, object?)> ExecuteAsync(byte source = 1, byte destination = 0, int timeout = 1000)
    {
        return ICcTalkCommand<object>.ExecuteWithAckAsync(receiver, new CcTalkDataBlock
        {
            Source = source,
            Destination = destination,
            Header = 164,
            Data = [enabled ? (byte)165 : (byte)0]
        }, timeout);
    }
}