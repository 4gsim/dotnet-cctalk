using System.Threading.Tasks;
using NUnit.Framework;
using CcTalk.Commands;
using System;
using System.Threading;

namespace CcTalk.Tests;

public class UsbSerialReceiverTests
{
    [Test]
    public async Task SimplePollAsync()
    {
        using (var receiver = new UsbSerialCcTalkReceiver("COM3"))
        {
            Console.WriteLine("Before SimplePollAsync" + Thread.CurrentThread.ManagedThreadId);
            var (err, _) = await new SimplePoll(receiver).ExecuteAsync();
            Assert.That(err, Is.Null);
            Console.WriteLine("After SimplePollAsync" + Thread.CurrentThread.ManagedThreadId);
        }
    }

    [Test]
    public async Task RequestCoinIdAsync()
    {
        using (var receiver = new UsbSerialCcTalkReceiver("COM3"))
        {
            var (err1, value1) = await new RequestCoinId(receiver, 1).ExecuteAsync();
            Assert.That(err1, Is.Null);
            Assert.That(value1.IntValue, Is.EqualTo(200));

            var (err2, value2) = await new RequestCoinId(receiver, 10).ExecuteAsync();
            Assert.That(err2, Is.Not.Null);
            Assert.That(value2.IsValid, Is.False);
        }
    }

    [Test]
    public async Task RunAsyncOperations()
    {
        Console.WriteLine($"[Main Thread] Starting on Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        // --- First await: Simulate I/O-bound operation ---
        Console.WriteLine($"[Before First Await] Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        // Task.Delay doesn't block the current thread; it returns to the caller.
        // The continuation after await Task.Delay() will likely resume on a thread pool thread,
        // as there's typically no SynchronizationContext captured in a console app.
        await Task.Delay(500);
        Console.WriteLine($"[After First Await - Task.Delay] Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        // --- Second await: Simulate CPU-bound operation using Task.Run ---
        Console.WriteLine($"[Before Second Await - Task.Run] Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        // Task.Run explicitly moves work to a thread pool thread.
        await Task.Run(() =>
        {
            Console.WriteLine($"  [Inside Task.Run] Performing CPU-bound work on Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(500); // Simulate some blocking CPU work
        });
        Console.WriteLine($"[After Second Await - Task.Run] Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        // --- Third await: With ConfigureAwait(false) ---
        Console.WriteLine($"[Before Third Await - ConfigureAwait(false)] Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        // await Task.Delay(500).ConfigureAwait(false) explicitly tells the runtime
        // not to capture the current SynchronizationContext (if any) and to resume
        // on any available thread pool thread. In a console app, this often behaves
        // similarly to not using ConfigureAwait(false) because there's no context to capture anyway.
        await Task.Delay(500).ConfigureAwait(false);
        Console.WriteLine($"[After Third Await - ConfigureAwait(false)] Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        // --- Fourth await: Example with a custom async method ---
        Console.WriteLine($"[Before Fourth Await - Custom Async] Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        await CustomAsyncMethod();
        Console.WriteLine($"[After Fourth Await - Custom Async] Thread ID: {Thread.CurrentThread.ManagedThreadId}");

        Console.WriteLine($"[End of Method] Thread ID: {Thread.CurrentThread.ManagedThreadId}");
    }

    public static async Task CustomAsyncMethod()
    {
        Console.WriteLine($"    [Inside CustomAsyncMethod - Start] Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        await Task.Delay(200);
        Console.WriteLine($"    [Inside CustomAsyncMethod - After Delay] Thread ID: {Thread.CurrentThread.ManagedThreadId}");
    }

    private Task RunOnThreadAsync(int delay)
    {
        Thread.Sleep(delay);
        return Task.CompletedTask;
    }

    [Test]
    public async Task TestAsync()
    {
        var task = Task.Run(async () => await RunOnThreadAsync(10000));
        var isFinished = await Task.WhenAny(task, Task.Delay(2000)) == task;
        Console.WriteLine(isFinished);
    }
}