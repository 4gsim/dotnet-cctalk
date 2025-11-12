namespace CcTalk;

public class UsbSerialCcTalkReceiver1 : CcTalkReceiver
{
    private readonly string _port;

    private ICcTalkConnection? _connection;

    public UsbSerialCcTalkReceiver1(string port, bool withEcho = true,
        CcTalkChecksumType checksumType = CcTalkChecksumType.Simple8) : base(withEcho, checksumType)
    {
        _port = port;
    }

    protected override ICcTalkConnection GetConnection()
    {
        _connection ??= new SerialCcTalkConnection(_port);

        return _connection;
    }

    public override void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
    }
}