using System.Threading.Tasks;
using NUnit.Framework;
using Dotnet.CcTalk;

namespace Dotnet.CcTalk.Tests;

public class CcTalkTests
{
    [Test]
    public async Task SendReceiveDataAsync()
    {
        var receiver = new JsonFileCcTalkReceiver("test.txt");
        var reply = new CcTalkDataBlock();
        var ret = await receiver.TryReceiveAsync(new CcTalkDataBlock()
        {
            Header = 1,
            Data = [1]
        }, ref reply);
        Assert.That(ret == null);
        Assert.That(reply.Header, Is.EqualTo(1));
    }
}