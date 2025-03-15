using System.Threading.Tasks;

namespace CcTalk;

public interface ICcTalkCommand<R>
{
    Task<(CcTalkError?, R?)> ExecuteAsync();

    static async Task<(CcTalkError?, object?)> ExecuteWithoutResponseAsync(ICcTalkReceiver receiver, byte header)
    {
        var reply = new CcTalkDataBlock();
        var ret = await receiver.TryReceiveAsync(new CcTalkDataBlock()
        {
            Header = header
        }, ref reply);
        if (ret != null)
        {
            return (ret, null);
        }
        return (null, null);
    }

    static async Task<(CcTalkError?, object?)> ExecuteWithAckAsync(ICcTalkReceiver receiver, CcTalkDataBlock command)
    {
        var reply = new CcTalkDataBlock();
        var ret = await receiver.TryReceiveAsync(command, ref reply);
        if (ret != null)
        {
            return (ret, null);
        }
        if (reply.Header == 5)
        {
            return (CcTalkError.FromMessage("Received NACK from device"), null);
        }
        return (null, null);
    }
}