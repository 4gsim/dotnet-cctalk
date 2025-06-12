using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CcTalk;

public class UsbSerialCcTalkReceiver1(string portName) : ICcTalkReceiver
{
    private readonly SemaphoreSlim _connectSemaphore = new(1, 1);
    private readonly Channel<TaskCompletionSource<byte[]>> _receiveChannel = Channel.CreateBounded<TaskCompletionSource<byte[]>>(1);

    private SerialPort? _serialPort;
    private Task? _receiveTask;
    private CancellationTokenSource? _receiveCts;

    public bool IsOpen => _serialPort != null && _serialPort.IsOpen;

    private async Task CancelReceiveTaskAsync()
    {
        _receiveCts?.Cancel();
        if (_receiveTask != null)
        {
            while (!_receiveTask.IsCanceled)
            {
                await Task.Delay(10);
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

    private CcTalkError? ReadBytes(byte[] buffer, int offset, int length)
    {
        for (var i = 0; i < length; i++)
        {
            try
            {
                var value = _serialPort.ReadByte();
                if (value == -1)
                {
                    Dispose();
                    return CcTalkError.FromMessage("EOF reached, connection is closed");
                }
                buffer[offset + i] = (byte)value;
            }
            catch (InvalidOperationException e)
            {
                Dispose();
                return CcTalkError.FromException(e);
            }
            catch (Exception e)
            {
                return CcTalkError.FromException(e);
            }
        }
         return null;
    }

    private (CcTalkError?, byte[]?) ReceiveBytes()
    {
        var buffer = new byte[256];
        var err = ReadBytes(buffer, 0, 2);
        if (err != null)
        {
            return (err, null);
        }
        var messageLength = 3 + buffer[1];
        err = ReadBytes(buffer, 2, messageLength);
        if (err != null)
        {
            return (err, null);
        }
        return (null, buffer);
    }

    private async Task DoReceiveAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var receiveTaskCompletion = await _receiveChannel.Reader.ReadAsync().ConfigureAwait(false);
        }
    }

    private async Task<CcTalkError?> CheckOpenAsync()
    {
        if (_serialPort == null)
        {
            await _connectSemaphore.WaitAsync();
            try
            {
                _serialPort = new SerialPort
                {
                    PortName = portName,
                    BaudRate = 9600,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    Encoding = Encoding.Unicode,

                    ReadTimeout = 50,
                    WriteTimeout = 500
                };
                await CancelReceiveTaskAsync();
                _serialPort.Open();
                return null;
            }
            catch (Exception e)
            {
                Dispose();
                return CcTalkError.FromException(e);
            }
            finally
            {
                _connectSemaphore.Release();
            }
        }
        return null;
    }

    public Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, bool withRetries = true)
    {
        CheckOpen();
    }

    public void Dispose()
    {
        _serialPort?.Dispose();
        _serialPort = null;
    }
}