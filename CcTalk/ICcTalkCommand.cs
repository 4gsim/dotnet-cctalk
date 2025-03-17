using System.Threading.Tasks;

namespace CcTalk;

public interface ICcTalkCommand<R>
{
    public const int Success = 1;

    Task<(CcTalkError?, R?)> ExecuteAsync();

    static async Task<(CcTalkError?, object?)> ExecuteWithAckAsync(ICcTalkReceiver receiver, CcTalkDataBlock command)
    {
        var (err, reply) = await receiver.ReceiveAsync(command);
        if (err != null)
        {
            return (err, null);
        }
        if (reply!.Value.Header == 5)
        {
            return (CcTalkError.FromMessage("Received NACK from device"), null);
        }
        return (null, Success);
    }
}