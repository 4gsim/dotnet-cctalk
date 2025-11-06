using System;
using System.IO.Ports;
using System.Text;

namespace CcTalk.Internal;

internal class UsbSerialConnection(string port) : ISerialConnection
{
    private readonly SerialPort _serialPort = new()
    {
        PortName = port,
        BaudRate = 9600,
        Parity = Parity.None,
        StopBits = StopBits.One,
        Handshake = Handshake.None,
        Encoding = Encoding.Unicode,
        WriteTimeout = 500
    };

    public void Open()
    {
        _serialPort.Open();
    }

    public void Write(byte[] bytes, int offset, int length)
    {
        _serialPort.Write(bytes, offset, length);
    }

    private void ReadBytes(byte[] buffer, int offset, int length)
    {
        for (var i = 0; i < length; i++)
        {
            var value = _serialPort.ReadByte();
            if (value == -1)
            {
                throw new InvalidOperationException("Serial port is closed");
            }

            buffer[offset + i] = (byte)value;
        }
    }

    public byte[] ReadBytes(int timeout)
    {
        var buffer = new byte[256];
        _serialPort.ReadTimeout = timeout;
        var firstByte = _serialPort.ReadByte();
        if (firstByte == -1)
        {
            throw new InvalidOperationException("Serial port is closed");
        }

        buffer[0] = (byte)firstByte;
        _serialPort.ReadTimeout = 50;
        ReadBytes(buffer, 1, 1);
        var messageLength = 3 + buffer[1];
        ReadBytes(buffer, 2, messageLength);
        return buffer;
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