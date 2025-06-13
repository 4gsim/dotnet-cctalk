using System;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using NLog;

namespace CcTalk;

public class UsbSerialCcTalkReceiver1(Func<IConnection> connectionFactory) : ICcTalkReceiver
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly SemaphoreSlim _commandSemaphore = new(1, 1);
    private readonly Channel<TaskCompletionSource<byte[]>> _receiveChannel = Channel.CreateBounded<TaskCompletionSource<byte[]>>(1);

    private IConnection? _connection;
    private Task? _receiveTask;
    private CancellationTokenSource? _receiveCts;

    public bool IsOpen => _connection != null && _connection.IsOpen;

    private async Task CancelReceiveTaskAsync()
    {
        _receiveCts?.Cancel();
        if (_receiveTask != null)
        {
            while (!_receiveTask.IsCompleted)
            {
                await Task.Delay(10).ConfigureAwait(false);
            }
            _receiveTask.Dispose();
            _receiveTask = null;
            _receiveCts?.Dispose();
            _receiveCts = null;
        }
    }

    private void StartReceiveTask()
    {
        if (_receiveTask == null)
        {
            _receiveCts = new CancellationTokenSource();
            _receiveTask = Task.Run(async () => await DoReceiveAsync(_receiveCts.Token), _receiveCts.Token);
        }
    }

    private byte[] WriteCommand(CcTalkDataBlock command)
    {
        return WithLogging("WriteCommand", () =>
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Serial port is not initialized");
            }
            var destination = (byte)0;
            var source = (byte)1;
            var messageLength = 5 + command.Data.Length;
            var bytes = new byte[messageLength];
            bytes[0] = destination;
            bytes[1] = command.DataLength;
            bytes[2] = source;
            bytes[3] = command.Header;
            var dataSum = (byte)0;
            for (var i = 0; i < command.DataLength; i++)
            {
                bytes[4 + i] = command.Data[i];
                dataSum = (byte)(dataSum + command.Data[i]);
            }
            bytes[messageLength - 1] = (byte)(256 - (byte)(destination + source + command.Header + command.DataLength + dataSum));
            _connection.Write(bytes, 0, messageLength);
            return bytes;
        });
    }

    private void ReadBytes(byte[] buffer, int offset, int length)
    {
        if (_connection == null)
        {
            throw new InvalidOperationException("Serial port is not initialized");
        }
        for (var i = 0; i < length; i++)
        {
            var value = _connection.ReadByte();
            if (value == -1)
            {
                throw new IOException("Serial port is closed");
            }
            buffer[offset + i] = (byte)value;
        }
    }

    private byte[] ReceiveBytes()
    {
        return WithLogging("ReceiveBytes", () =>
        {
            var buffer = new byte[256];
            ReadBytes(buffer, 0, 2);
            var messageLength = 3 + buffer[1];
            ReadBytes(buffer, 2, messageLength);
            return buffer;
        });
    }

    private async Task DoReceiveAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var receiveTaskCompletion = await _receiveChannel.Reader.ReadAsync(token).ConfigureAwait(false);
            try
            {
                var bytes = ReceiveBytes();
                receiveTaskCompletion.TrySetResult(bytes);
            }
            catch (Exception e)
            {
                receiveTaskCompletion.TrySetException(e);
            }
        }
    }

    private Task CheckOpenAsync()
    {
        return WithLoggingAsync("CheckOpenAsync", async () =>
        {
            if (_connection == null)
            {
                _connection = connectionFactory();
                await CancelReceiveTaskAsync().ConfigureAwait(false);
                _connection.Open();
                StartReceiveTask();
            }
        });
    }

    private static (CcTalkError?, CcTalkDataBlock?) Deserialize(byte[] bytes)
    {
        return WithLogging<(CcTalkError?, CcTalkDataBlock?)>("Deserialize", () =>
        {
            try
            {
                if (bytes[0] != 1)
                {
                    return (CcTalkError.FromMessage($"Received wrong destination {bytes[0]}"), null);
                }
                var data = new CcTalkDataBlock
                {
                    Header = bytes[3],
                    Data = new byte[bytes[1]]
                };
                var sum = (byte)(bytes[0] + bytes[1] + bytes[2] + bytes[3] + bytes[4 + bytes[1]]);
                for (var i = 0; i < bytes[1]; i++)
                {
                    data.Data[i] = bytes[4 + i];
                    sum = (byte)(sum + bytes[4 + i]);
                }
                if (sum != 0)
                {
                    return (CcTalkError.FromMessage("Incorrect checksum"), null);
                }
                return (null, data);
            }
            catch (Exception e)
            {
                return (CcTalkError.FromException(e), null);
            }
        });
    }

    public Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, bool withRetries = true)
    {
        return WithLoggingAsync("ReceiveAsync", async () =>
        {
            await _commandSemaphore.WaitAsync();
            try
            {
                await CheckOpenAsync().ConfigureAwait(false);
                var request = WriteCommand(command);
                var echoReadTcs = new TaskCompletionSource<byte[]>();
                await _receiveChannel.Writer.WriteAsync(echoReadTcs).ConfigureAwait(false);
                var echo = await echoReadTcs.Task.ConfigureAwait(false);
                for (var i = 0; i < request!.Length; i++)
                {
                    if (request![i] != echo![i])
                    {
                        return (CcTalkError.FromMessage("Request and echo do not match"), null);
                    }
                }
                var responseReadTcs = new TaskCompletionSource<byte[]>();
                await _receiveChannel.Writer.WriteAsync(responseReadTcs).ConfigureAwait(false);
                var response = await responseReadTcs.Task.ConfigureAwait(false);
                return Deserialize(response);
            }
            catch (Exception e)
            {
                await DisposeAsync().ConfigureAwait(false);
                return (CcTalkError.FromException(e), null);
            }
            finally
            {
                _commandSemaphore.Release();
            }
        });
    }

    private static T WithLogging<T>(string name, Func<T> func)
    {
        Logger.Trace("TID:{tid} (ENT) {name}", Environment.CurrentManagedThreadId, name);
        try
        {
            return func();
        }
        finally
        {
            Logger.Trace("TID:{tid} (EXT) {name}", Environment.CurrentManagedThreadId, name);
        }
    }

    private static async Task WithLoggingAsync(string name, Func<Task> func)
    {
        Logger.Trace("TID:{tid} (ENT) {name}", Environment.CurrentManagedThreadId, name);
        try
        {
            await func();
        }
        finally
        {
            Logger.Trace("TID:{tid} (EXT) {name}", Environment.CurrentManagedThreadId, name);
        }
    }

    private static async Task<T> WithLoggingAsync<T>(string name, Func<Task<T>> func)
    {
        Logger.Trace("TID:{tid} (ENT) {name}", Environment.CurrentManagedThreadId, name);
        try
        {
            return await func();
        }
        finally
        {
            Logger.Trace("TID:{tid} (EXT) {name}", Environment.CurrentManagedThreadId, name);
        }
    }

    private async Task DisposeAsync()
    {
        await CancelReceiveTaskAsync().ConfigureAwait(false);
        _connection?.Dispose();
        _connection = null;
    }

    public void Dispose()
    {
        _ = Task.Run(DisposeAsync);
    }
}