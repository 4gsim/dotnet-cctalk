using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CcTalk;

internal record ReceiveTaskContext(TaskCompletionSource<byte[]> Tcs, int Timeout);

public class SerialReceiver(string port, bool withEcho) : ISerialReceiver
{
    private readonly Channel<ReceiveTaskContext> _receiveChannel = Channel.CreateBounded<ReceiveTaskContext>(1);

    private Task? _receiveTask;
    private CancellationTokenSource? _receiveCts;
    private SerialPort? _serialPort;
    private bool _disposed;
    
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
    
    private static void ReadBytes(SerialPort serialPort, byte[] buffer, int offset, int length)
    {
        for (var i = 0; i < length; i++)
        {
            var value = serialPort.ReadByte();
            if (value == -1)
            {
                throw new InvalidOperationException("Serial port is closed");
            }

            buffer[offset + i] = (byte)value;
        }
    }
    
    private byte[] ReceiveBytes(int timeout)
    {
        if (_serialPort == null)
        {
            throw new InvalidOperationException("Serial port is not initialized");
        }
        var buffer = new byte[256];
        _serialPort.ReadTimeout = timeout;
        var firstByte = _serialPort.ReadByte();
        if (firstByte == -1)
        {
            throw new InvalidOperationException("Serial port is closed");
        }

        buffer[0] = (byte)firstByte;
        _serialPort.ReadTimeout = 50;
        ReadBytes(_serialPort, buffer, 1, 1);
        var messageLength = 3 + buffer[1];
        ReadBytes(_serialPort, buffer, 2, messageLength);
        return buffer;
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
    
    private async Task CheckOpenAsync()
    {
        if (_serialPort == null)
        {
            _serialPort = _serialPort = new SerialPort
            {
                PortName = port,
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                Encoding = Encoding.Unicode,
                WriteTimeout = 500
            };
            await CancelReceiveTaskAsync().ConfigureAwait(false);
            _serialPort.Open();
            StartReceiveTask();
        }
    }
    
    private byte[] WriteCommand(byte[] command)
    {
        if (_serialPort == null)
        {
            throw new InvalidOperationException("Serial port is not initialized");
        }
        _serialPort.Write(command, 0, command.Length);
        return command;
    }

    public async Task<byte[]> ReceiveAsync(byte[] command, int timeout)
    {
        await CheckOpenAsync().ConfigureAwait(false);
        var request = WriteCommand(command);
        if (withEcho)
        {
            var echoReadCtx = new ReceiveTaskContext(new TaskCompletionSource<byte[]>(), timeout);
            await _receiveChannel.Writer.WriteAsync(echoReadCtx).ConfigureAwait(false);
            var echo = await echoReadCtx.Tcs.Task.ConfigureAwait(false);
            if (request.Where((b, i) => b != echo[i]).Any())
            {
                throw new Exception("Request and echo do not match");
            }
        }

        var responseReadCtx = new ReceiveTaskContext(new TaskCompletionSource<byte[]>(), timeout);
        await _receiveChannel.Writer.WriteAsync(responseReadCtx).ConfigureAwait(false);
        return await responseReadCtx.Tcs.Task.ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }
        
        await CancelReceiveTaskAsync().ConfigureAwait(false);

        if (_serialPort != null)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort.Dispose();
            _serialPort = null;
        }

        _disposed = true;
    }
}