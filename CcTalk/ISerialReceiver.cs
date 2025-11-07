using System;
using System.Threading.Tasks;

namespace CcTalk;

public interface ISerialReceiver : IAsyncDisposable
{
    Task<byte[]> ReceiveAsync(byte[] command, int timeout);
}