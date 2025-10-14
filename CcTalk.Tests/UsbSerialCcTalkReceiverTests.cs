using System;
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
            var (err1, _) = await new SimplePoll(receiver).ExecuteAsync(1, 2, timeout: 5000);
            Assert.That(err1, Is.Null);
            
            var (err2, _) = await new ModifyMasterInhibitStatus(receiver, true).ExecuteAsync(1, 2, timeout: 5000);
            Assert.That(err2, Is.Null);
            
            var (err3, _) = await new ModifyInhibitStatus(receiver, 65535).ExecuteAsync(1, 2, timeout: 5000);
            Assert.That(err3, Is.Null);

            while (true)
            {
                var (err4, response) = await new ReadBufferedCreditOrErrorCodes(receiver).ExecuteAsync(1, 2, timeout: 5000);
                Assert.That(err4, Is.Null);

                Console.WriteLine(response);
                await Task.Delay(1000);
            }
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
            
            var (err2, value2) = await new RequestCoinId(receiver, 3).ExecuteAsync(1, 2);
            Assert.That(err2, Is.Null);
            Assert.That(value2.Id, Is.Null);
        }
    }
}