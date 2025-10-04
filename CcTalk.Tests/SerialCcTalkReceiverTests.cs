using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CcTalk.Commands;
using CcTalk.Internal;
using NUnit.Framework;

namespace CcTalk.Tests;

public class MockSerialConnection : ISerialConnection
{
    private readonly List<byte[]> _data = [];
    private int _dataIdx;

    public MockSerialConnection(byte[] response)
    {
        _data.Add(response);
    }

    public void Open()
    {

    }

    public void Write(byte[] bytes, int offset, int length)
    {
        _data.Insert(0, bytes);
    }

    public byte[] ReadBytes(int timeout)
    {
        if (_dataIdx < _data.Count)
        {
            return _data[_dataIdx++];
        }
        throw new InvalidOperationException("No more data");
    }

    public void Dispose()
    {

    }
}

public class MockSerialCcTalkReceiver(byte[] response) : SerialCcTalkReceiver
{
    protected override ISerialConnection BuildConnection()
    {
        return new MockSerialConnection(response);
    }
}

public class SerialCcTalkReceiverTests
{
    [Test]
    public async Task ResetDeviceAsync()
    {
        using (var receiver = new MockSerialCcTalkReceiver([0x01, 0x00, 0x02, 0x00, 0xfd]))
        {
            var (err, _) = await new ResetDevice(receiver).ExecuteAsync(source: 1, destination: 0);
            Assert.That(err, Is.Null);
        }
    }
}