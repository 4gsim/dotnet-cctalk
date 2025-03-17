using System;
using System.Threading.Tasks;

namespace CcTalk;

public interface ICcTalkReceiver : IDisposable
{
    Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, bool withRetries = true);
}