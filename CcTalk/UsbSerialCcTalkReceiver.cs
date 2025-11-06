using CcTalk.Internal;

namespace CcTalk;

/// <summary>
/// Represents a CcTalk receiver that communicates over a USB serial port.
/// </summary>
/// <remarks>
/// This class extends <see cref="SerialCcTalkReceiver"/> and provides a USB-specific serial connection.
/// </remarks>
public class UsbSerialCcTalkReceiver(
    string port,
    bool withEcho = true,
    CcTalkChecksumType checksumType = CcTalkChecksumType.Simple8
) : SerialCcTalkReceiver(withEcho, checksumType)
{
    protected override ISerialConnection BuildConnection()
    {
        return new UsbSerialConnection(port);
    }
}