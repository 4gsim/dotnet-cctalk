using System;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace CcTalk;

public class SerialCcTalkConnection : ICcTalkConnection
{
    private readonly SerialPort _serialPort;
    private readonly SerialReadOperation _readOperation;
    
    private int _readTimeout;

    public SerialCcTalkConnection(string port)
    {
        _serialPort = new SerialPort
        {
            PortName = port,
            BaudRate = 9600,
            Parity = Parity.None,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
            Encoding = Encoding.Unicode,
            WriteTimeout = 500
        };
        _serialPort.Open();
        _readOperation = new SerialReadOperation(_serialPort);
    }

    public void Write(byte[] input)
    {
        _serialPort.Write(input, 0, input.Length);
    }

    public async ValueTask<byte> ReadByteAsync(int timeout)
    {
        if (_readTimeout != timeout)
        {
            _serialPort.ReadTimeout = timeout;
            _readTimeout = timeout;
        }

        return await _readOperation.ReadAsync();
    }

    public void Dispose()
    {
        if (_serialPort.IsOpen)
        {
            _serialPort.Close();
        }

        _serialPort.Dispose();
    }
}