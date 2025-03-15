using System.Threading.Tasks;

namespace CcTalk;

public interface ICcTalkReceiver
{
    Task<CcTalkError?> TryReceiveAsync(CcTalkDataBlock command, ref CcTalkDataBlock reply);
}