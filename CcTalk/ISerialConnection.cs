using System;

namespace CcTalk;

public interface ISerialConnection : IDisposable
{
    void Open();

    void Write(byte[] bytes, int offset, int length);

    byte[] ReadBytes(int timeout);
}