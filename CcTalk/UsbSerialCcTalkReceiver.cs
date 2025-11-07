using System;
using System.Threading;
using System.Threading.Tasks;

namespace CcTalk;

/// <summary>
/// Represents a CcTalk receiver that communicates over a USB serial port.
/// </summary>
/// <remarks>
/// This class extends <see cref="SerialCcTalkReceiver"/> and provides a USB-specific serial connection.
/// </remarks>
public class UsbSerialCcTalkReceiver(string port, bool withEcho = true, CcTalkChecksumType checksumType = CcTalkChecksumType.Simple8) : ICcTalkReceiver
{
    private readonly SemaphoreSlim _commandSemaphore = new(1, 1);
    
    private ISerialReceiver? _serialReceiver;
    private SerialCcTalkReceiver? _receiver;
    private bool _disposed;
    
    public async Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, int timeout)
    {
        await _commandSemaphore.WaitAsync();
        if (_receiver == null)
        {
            _serialReceiver = new SerialReceiver(port, withEcho);
            _receiver = new SerialCcTalkReceiver(_serialReceiver, checksumType);
        }
        try
        {
            return await _receiver.ReceiveAsync(command, timeout);
        }
        catch (Exception e)
        {
            await DisposeReceiverAsync().ConfigureAwait(false);
            return (CcTalkError.FromException(e), null);
        }
        finally
        {
            _commandSemaphore.Release();
        }
    }

    private async Task DisposeReceiverAsync()
    {
        if (_serialReceiver != null)
        {
            await _serialReceiver.DisposeAsync();
        }
        _serialReceiver = null;
        _receiver = null;
    }
    
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _ = DisposeReceiverAsync();
        _disposed = true;
    }
}