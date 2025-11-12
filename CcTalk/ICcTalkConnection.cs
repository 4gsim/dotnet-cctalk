using System;
using System.Threading.Tasks;

namespace CcTalk;

public interface ICcTalkConnection : IDisposable
{
    void Write(byte[] input);

    ValueTask<byte> ReadByteAsync(int timeout);
}