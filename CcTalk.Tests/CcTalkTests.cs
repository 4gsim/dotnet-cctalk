using System.Threading.Tasks;
using NUnit.Framework;

namespace CcTalk.Tests;

public class CcTalkTests
{
    [Test]
    public async Task SendReceiveDataAsync()
    {
        var receiver = new JsonFileCcTalkReceiver("test.txt");
        var (err, reply) = await receiver.ReceiveAsync(new CcTalkDataBlock()
        {
            Header = 1,
            Data = [1]
        }, false);
        Assert.That(err == null);
        Assert.That(reply!.Value.Header, Is.EqualTo(1));
    }
}