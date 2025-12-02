using CcTalk.Serial;

namespace CcTalk;

/// <summary>
/// Represents a CcTalk receiver that communicates over a USB serial port.
/// </summary>
/// <remarks>
/// This class extends <see cref="CcTalkReceiver"/> and provides a USB-specific CcTalk receiver.
/// </remarks>
public class UsbSerialCcTalkReceiver(
    string port,
    bool withEcho = true,
    CcTalkChecksumType checksumType = CcTalkChecksumType.Simple8)
    : CcTalkReceiver(withEcho, checksumType)
{
    private ISerialConnection? _connection;

    protected override ISerialConnection GetConnection()
    {
        _connection ??= new SerialConnection(port);

        return _connection;
    }

    public override void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
    }
}