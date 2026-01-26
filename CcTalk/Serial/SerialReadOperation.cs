using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace CcTalk.Serial;

internal class SerialReadOperation(SerialPort serialPort) : IValueTaskSource<byte>
{
    private readonly SerialPort _serialPort = serialPort;

    private ManualResetValueTaskSourceCore<byte> _delegate = new()
    {
        RunContinuationsAsynchronously = true,
    };

    private bool _isRunning;

    public byte GetResult(short token)
    {
        try
        {
            return _delegate.GetResult(token);
        }
        finally
        {
            _delegate.Reset();
            _isRunning = false;
        }
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        return _delegate.GetStatus(token);
    }

    public void OnCompleted(Action<object> continuation, object state, short token,
        ValueTaskSourceOnCompletedFlags flags)
    {
        _delegate.OnCompleted(continuation, state, token, flags);
    }

    public ValueTask<byte> ReadAsync()
    {
        if (_isRunning)
            throw new InvalidOperationException("Operation already in progress");

        _isRunning = true;
        ThreadPool.QueueUserWorkItem(static state =>
        {
            var op = (SerialReadOperation)state!;
            try
            {
                var result = op._serialPort.ReadByte();
                if (result == -1)
                {
                    throw new InvalidOperationException("Serial port is closed");
                }

                op._delegate.SetResult((byte)result);
            }
            catch (Exception ex)
            {
                op._delegate.SetException(ex);
            }
        }, this);

        return new ValueTask<byte>(this, _delegate.Version);
    }
}