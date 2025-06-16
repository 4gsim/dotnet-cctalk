using System;
using System.Threading.Tasks;

namespace CcTalk;

public interface ICcTalkReceiver : IDisposable
{
    Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, int timeout = 1000);
}