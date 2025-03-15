using System.Threading.Tasks;

namespace Dotnet.CcTalk;

public interface ICcTalkReceiver
{
    Task<CcTalkError?> TryReceiveAsync(CcTalkDataBlock command, ref CcTalkDataBlock reply);
}