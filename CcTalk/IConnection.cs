using System;

namespace CcTalk;

public interface IConnection : IDisposable
{
    public bool IsOpen { get; }

    public void Open();

    public void Write(byte[] bytes, int offset, int length);

    public int ReadByte();
}