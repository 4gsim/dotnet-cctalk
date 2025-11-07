using System.Threading.Tasks;
using CcTalk.Commands;
using NUnit.Framework;

namespace CcTalk.Tests;

public class MockSerialReceiver(byte[] data) : ISerialReceiver
{
    public Task<byte[]> ReceiveAsync(byte[] command, int timeout)
    {
        return Task.FromResult(data);
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}

public class SerialCcTalkReceiverTests
{
    [Test]
    public async Task ResetDeviceAsync()
    {
        using (var receiver = new SerialCcTalkReceiver(new MockSerialReceiver([0x01, 0x00, 0x02, 0x00, 0xfd])))
        {
            var (err, _) = await new ResetDevice(receiver).ExecuteAsync(source: 1, destination: 0);
            Assert.That(err, Is.Null);
        }
    }
}