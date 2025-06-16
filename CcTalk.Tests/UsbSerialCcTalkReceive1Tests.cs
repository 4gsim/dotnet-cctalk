using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using CcTalk.Commands;
using NUnit.Framework;

namespace CcTalk.Tests;

public class MockConnection : ISerialConnection
{
    private readonly List<byte[]> _data = [];
    private int _idx = 0;
    private int _dataIdx = 0;

    public MockConnection(byte[] response)
    {
        _data.Add(response);
    }

    public bool IsOpen => true;

    public void Open()
    {

    }

    public int ReadByte()
    {
        if (_dataIdx < _data.Count)
        {
            var bytes = _data[_dataIdx];
            if (_idx < bytes.Length)
            {
                return bytes[_idx++];
            }
            _idx = 0;
            _dataIdx++;
            return ReadByte();
        }
        return -1;
    }

    public void Write(byte[] bytes, int offset, int length)
    {
        _data.Insert(0, bytes);
    }

    public void Dispose()
    {

    }
}

public class SerialConnection : ISerialConnection
{
    private readonly SerialPort _serialPort;

    public bool IsOpen => _serialPort.IsOpen;

    public SerialConnection(string port)
    {
        _serialPort = new SerialPort
        {
            PortName = port,
            BaudRate = 9600,
            Parity = Parity.None,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
            Encoding = Encoding.Unicode,

            ReadTimeout = 50,
            WriteTimeout = 500
        };
    }

    public void Open()
    {
        _serialPort.Open();
    }

    public int ReadByte()
    {
        return _serialPort.ReadByte();
    }

    public void Write(byte[] bytes, int offset, int length)
    {
        _serialPort.Write(bytes, offset, length);
    }

    public void Dispose()
    {
        _serialPort.Dispose();
    }
}

public class UsbSerialCcTalkReceiver1Test
{
    [Test]
    public async Task ResetDeviceAsync()
    {
        var connection = new MockConnection([0x01, 0x00, 0x02, 0x00, 0xfd]);
        using (var receiver = new UsbSerialCcTalkReceiver1(() => connection))
        {
            var (err, _) = await new ResetDevice(receiver).ExecuteAsync();
            Assert.That(err, Is.Null);
        }
        connection = new MockConnection([0x01, 0x00, 0x02, 0x00, 0xfd]);
        using (var receiver = new UsbSerialCcTalkReceiver1(() => connection))
        {
            var (err, _) = await new ResetDevice(receiver).ExecuteAsync();
            Assert.That(err, Is.Null);
        }
    }

    [Test]
    public async Task SimplePollAsync()
    {
        var connection = new SerialConnection("COM3");
        using (var receiver = new UsbSerialCcTalkReceiver1(() => connection))
        {
            var (err, _) = await new SimplePoll(receiver).ExecuteAsync();
            Assert.That(err, Is.Null);
        }
    }
}