using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using NLog;

[assembly: InternalsVisibleTo("CcTalk.Tests")]

namespace CcTalk.Internal;

internal record ReceiveTaskContext(TaskCompletionSource<byte[]> Tcs, int Timeout);

public abstract class SerialCcTalkReceiver : ICcTalkReceiver
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly ushort[] CrcTable;

    private readonly bool _withEcho;
    private readonly CcTalkChecksumType _checksumType;
    private readonly SemaphoreSlim _commandSemaphore = new(1, 1);
    private readonly Channel<ReceiveTaskContext> _receiveChannel = Channel.CreateBounded<ReceiveTaskContext>(1);

    private ISerialConnection? _connection;
    private Task? _receiveTask;
    private CancellationTokenSource? _receiveCts;

    internal SerialCcTalkReceiver(bool withEcho = true, CcTalkChecksumType checksumType = CcTalkChecksumType.Simple8)
    {
        _withEcho = withEcho;
        _checksumType = checksumType;
    }

    static SerialCcTalkReceiver()
    {
        CrcTable = BuildCrcTable();
    }

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
        if (_receiveTask != null) return;

        _receiveCts = new CancellationTokenSource();
        _receiveTask = Task.Run(async () => await DoReceiveAsync(_receiveCts.Token), _receiveCts.Token);
    }

    private byte[] WriteCommand(CcTalkDataBlock command)
    {
        return WithLogging("WriteCommand", () =>
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Serial port is not initialized");
            }

            var messageLength = 5 + command.Data.Length;
            var bytes = new byte[messageLength];
            bytes[0] = command.Destination;
            bytes[1] = command.DataLength;
            bytes[2] = command.Source;
            bytes[3] = command.Header;
            var dataSum = (byte)0;
            for (var i = 0; i < command.DataLength; i++)
            {
                bytes[4 + i] = command.Data[i];
                if (_checksumType == CcTalkChecksumType.Simple8)
                {
                    dataSum = (byte)(dataSum + command.Data[i]);
                }
            }

            if (_checksumType == CcTalkChecksumType.Simple8)
            {
                bytes[messageLength - 1] =
                    (byte)(256 - (byte)(command.Source + command.Destination + command.Header + command.DataLength +
                                        dataSum));
            }
            else
            {
                var crc = CalculateCrc16Checksum(bytes);
                bytes[2] = (byte)crc;
                bytes[messageLength - 1] = (byte)(crc >> 8);
            }

            _connection.Write(bytes, 0, messageLength);
            return bytes;
        });
    }

    private byte[] ReceiveBytes(int timeout)
    {
        return WithLogging("ReceiveBytes", () =>
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Serial port is not initialized");
            }

            return _connection.ReadBytes(timeout);
        });
    }

    private async Task DoReceiveAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var receiveTaskContext = await _receiveChannel.Reader.ReadAsync(token).ConfigureAwait(false);
            try
            {
                var bytes = ReceiveBytes(receiveTaskContext.Timeout);
                receiveTaskContext.Tcs.TrySetResult(bytes);
            }
            catch (Exception e)
            {
                receiveTaskContext.Tcs.TrySetException(e);
            }
        }
    }

    protected abstract ISerialConnection BuildConnection();

    private Task CheckOpenAsync()
    {
        return WithLoggingAsync("CheckOpenAsync", async () =>
        {
            if (_connection == null)
            {
                _connection = BuildConnection();
                await CancelReceiveTaskAsync().ConfigureAwait(false);
                _connection.Open();
                StartReceiveTask();
            }
        });
    }

    private (CcTalkError?, CcTalkDataBlock?) Deserialize(byte[] bytes)
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
                if (_checksumType == CcTalkChecksumType.Simple8)
                {
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
                }
                else
                {
                    var crc = CalculateCrc16Checksum(bytes);
                    if (bytes[2] != (byte)crc)
                    {
                        return (CcTalkError.FromMessage("Incorrect checksum"), null);
                    }

                    if (bytes[^1] != (byte)(crc >> 8))
                    {
                        return (CcTalkError.FromMessage("Incorrect checksum"), null);
                    }
                }

                return (null, data);
            }
            catch (Exception e)
            {
                return (CcTalkError.FromException(e), null);
            }
        });
    }

    public Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, int timeout)
    {
        return WithLoggingAsync("ReceiveAsync", async () =>
        {
            await _commandSemaphore.WaitAsync();
            try
            {
                await CheckOpenAsync().ConfigureAwait(false);
                var request = WriteCommand(command);
                if (_withEcho)
                {
                    var echoReadCtx = new ReceiveTaskContext(new TaskCompletionSource<byte[]>(), timeout);
                    await _receiveChannel.Writer.WriteAsync(echoReadCtx).ConfigureAwait(false);
                    var echo = await echoReadCtx.Tcs.Task.ConfigureAwait(false);
                    for (var i = 0; i < request.Length; i++)
                    {
                        if (request[i] != echo[i])
                        {
                            return (CcTalkError.FromMessage("Request and echo do not match"), null);
                        }
                    }
                }

                var responseReadCtx = new ReceiveTaskContext(new TaskCompletionSource<byte[]>(), timeout);
                await _receiveChannel.Writer.WriteAsync(responseReadCtx).ConfigureAwait(false);
                var response = await responseReadCtx.Tcs.Task.ConfigureAwait(false);
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

    private static ushort[] BuildCrcTable()
    {
        var table = new ushort[256];
        for (var i = 0; i < 256; ++i)
        {
            var crc = i << 8 ^ 0;
            for (var j = 0; j < 8; ++j)
            {
                if ((crc & 0x8000) == 0x8000)
                {
                    crc = (crc << 1) ^ 0x1021; // 0001.0000 0010.0001 = x^12 + x^5 + 1 ( + x^16 )
                }
                else
                {
                    crc <<= 1;
                }
            }

            table[i] = (ushort)crc;
        }

        return table;
    }

    private static ushort CalculateCrc16Checksum(byte[] bytes)
    {
        ushort crc = 0;
        for (var i = 0; i < bytes.Length - 1; ++i)
        {
            if (i == 2)
            {
                continue; // Skip the least significant byte of the checksum
            }

            crc = (ushort)((ushort)(crc << 8) ^ CrcTable[(byte)((crc >> 8) ^ bytes[i])]);
        }

        return crc;
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