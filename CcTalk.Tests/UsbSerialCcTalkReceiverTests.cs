using System;
using System.Threading.Tasks;
using CcTalk.Commands;
using NLog;
using NUnit.Framework;

namespace CcTalk.Tests;

public class UsbSerialCcTalkReceiverTests
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    [Test]
    public async Task SimplePollAsync()
    {
        using (var receiver = new UsbSerialCcTalkReceiver("COM4"))
        {
            var (err1, _) = await new SimplePoll(receiver).ExecuteAsync(1, 2, timeout: 5000);
            Assert.That(err1, Is.Null);

            var (err2, _) = await new ModifyMasterInhibitStatus(receiver, false).ExecuteAsync(1, 2, timeout: 5000);
            Assert.That(err2, Is.Null);

            var (err3, _) = await new ModifyInhibitStatus(receiver, 65535).ExecuteAsync(1, 2, timeout: 5000);
            Assert.That(err3, Is.Null);

            var i = 0;
            var counter = -1;
            while (i < 60)
            {
                var (err4, response) =
                    await new ReadBufferedCreditOrErrorCodes(receiver).ExecuteAsync(1, 2, timeout: 5000);
                Assert.That(err4, Is.Null);

                Logger.Info("counter {counter}", response!.Value.Counter);
                if (counter == -1)
                {
                    counter = response!.Value.Counter;
                }
                else
                {
                    var eventsFromThisPoll = response!.Value.Counter < counter
                        ? byte.MaxValue - counter + response!.Value.Counter //Overflow from 255 back to 0
                        : response!.Value.Counter - counter; //Normal iteration
                    counter = response!.Value.Counter;
                    foreach (var ev in response!.Value.Events)
                    {
                        if (eventsFromThisPoll-- > 0)
                        {
                            Logger.Info("event {event}", ev);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                await Task.Delay(200);
                i++;
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