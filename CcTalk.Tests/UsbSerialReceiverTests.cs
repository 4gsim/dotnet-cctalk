using System.Threading.Tasks;
using NUnit.Framework;
using CcTalk.Commands;

namespace CcTalk.Tests;

public class UsbSerialReceiverTests
{
    [Test]
    public async Task SimplePollAsync()
    {
        using (var receiver = new UsbSerialCcTalkReceiver("COM3"))
        {
            var (err, _) = await new SimplePoll(receiver).ExecuteAsync();
            Assert.That(err, Is.Null);
        }
    }

    [Test]
    public async Task RequestCoinIdAsync()
    {
        using (var receiver = new UsbSerialCcTalkReceiver("COM3"))
        {
            var (err, value) = await new RequestCoinId(receiver, 2).ExecuteAsync();
            Assert.That(err, Is.Null);
            Assert.That(value, Is.EqualTo("CH500A"));
        }
    }
}