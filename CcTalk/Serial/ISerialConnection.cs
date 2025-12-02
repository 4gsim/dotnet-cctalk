using System;
using System.Threading.Tasks;

namespace CcTalk.Serial;

public interface ISerialConnection : IDisposable
{
    void Write(byte[] input);

    ValueTask<byte> ReadByteAsync(int timeout);
}