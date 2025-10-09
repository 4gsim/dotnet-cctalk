using System.Threading.Tasks;
using CcTalk.Commands;
using NUnit.Framework;

namespace CcTalk.Tests;

public class UsbSerialCcTalkReceiverTests
{
    [Test]
    public async Task SimplePollAsync()
    {
        using (var receiver = new UsbSerialCcTalkReceiver("COM4"))
        {
            var (err, _) = await new SimplePoll(receiver).ExecuteAsync(1, 2, timeout: 5000);
            Assert.That(err, Is.Null);
        }
    }

    [Test]
    public async Task RequestCoinIdAsync()
    {
        using (var receiver = new UsbSerialCcTalkReceiver("COM4"))
        {
            var (err1, value1) = await new RequestCoinId(receiver, 1).ExecuteAsync(1, 2);
            Assert.That(err1, Is.Null);
            Assert.That(value1.IntValue, Is.EqualTo(200));

            var (err2, value2) = await new RequestCoinId(receiver, 10).ExecuteAsync(1, 2);
            Assert.That(err2, Is.Not.Null);
            Assert.That(value2.IsValid, Is.False);
        }
    }
}