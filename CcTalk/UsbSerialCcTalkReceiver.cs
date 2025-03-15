using System;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace CcTalk;

public class UsbSerialCcTalkReceiver : ICcTalkReceiver, IDisposable
{
    private readonly SerialPort _serialPort;

    public UsbSerialCcTalkReceiver(string portName)
    {
        _serialPort = new SerialPort
        {
            PortName = portName,
            BaudRate = 9600,
            Parity = Parity.None,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
            Encoding = Encoding.Unicode,

            ReadTimeout = 500,
            WriteTimeout = 500
        };

        _serialPort.Open();
    }

    private (CcTalkError?, byte[]?) WriteCommand(CcTalkDataBlock command)
    {
        try
        {
            var destination = (byte)0;
            var source = (byte)1;
            var messageLength = 5 + command.Data.Length;
            var bytes = new byte[messageLength];
            bytes[0] = destination;
            bytes[1] = command.DataLength;
            bytes[2] = source;
            bytes[3] = command.Header;
            var dataSum = (byte)0;
            for (var i = 0; i < command.DataLength; i++)
            {
                bytes[4 + i] = command.Data[i];
                dataSum = (byte)(dataSum + command.Data[i]);
            }
            bytes[messageLength - 1] = (byte)(256 - (byte)(destination + source + command.Header + command.DataLength + dataSum));
            _serialPort.Write(bytes, 0, messageLength);
            return (null, bytes);
        }
        catch (Exception e)
        {
            return (CcTalkError.FromException(e), null);
        }
    }

    private (CcTalkError?, byte[]?) ReceiveBytes()
    {
        try
        {
            var bytes = new byte[256];
            if (_serialPort.Read(bytes, 0, 1) != 1)
            {
                return (CcTalkError.FromMessage("Cannot read destination address"), null);
            }
            if (_serialPort.Read(bytes, 1, 1) != 1)
            {
                return (CcTalkError.FromMessage("Cannot read data length"), null);
            }
            var messageLength = 3 + bytes[1];
            if (_serialPort.Read(bytes, 2, messageLength) != messageLength)
            {
                return (CcTalkError.FromMessage("Cannot read message, incorrect length"), null);
            }
            return (null, bytes);
        }
        catch (Exception e)
        {
            return (CcTalkError.FromException(e), null);
        }
    }

    private static CcTalkError? Deserialize(byte[] bytes, ref CcTalkDataBlock data)
    {
        try
        {
            if (bytes[0] != 1)
            {
                return CcTalkError.FromMessage($"Received wrong destination {bytes[0]}");
            }
            data.Header = bytes[3];
            data.Data = new byte[bytes[1]];
            var sum = (byte)(bytes[0] + bytes[1] + bytes[2] + bytes[3] + bytes[4 + bytes[1]]);
            for (var i = 0; i < bytes[1]; i++)
            {
                data.Data[i] = bytes[4 + i];
                sum = (byte)(sum + bytes[4 + i]);
            }
            if (sum != 0)
            {
                return CcTalkError.FromMessage("Incorrect checksum");
            }
            return null;
        }
        catch (Exception e)
        {
            return CcTalkError.FromException(e);
        }
    }

    public Task<CcTalkError?> TryReceiveAsync(CcTalkDataBlock command, ref CcTalkDataBlock reply)
    {
        var (err1, request) = WriteCommand(command);
        if (err1 != null)
        {
            return Task.FromResult(err1);
        }
        var (err2, echo) = ReceiveBytes();
        if (err2 != null)
        {
            return Task.FromResult(err2);
        }
        for (var i = 0; i < request!.Length; i++)
        {
            if (request![i] != echo![i])
            {
                return Task.FromResult<CcTalkError?>(CcTalkError.FromMessage("Request and echo do not match"));
            }
        }
        var (err3, response) = ReceiveBytes();
        if (err3 != null)
        {
            return Task.FromResult(err3);
        }
        return Task.FromResult(Deserialize(response!, ref reply));
    }

    public void Dispose()
    {
        _serialPort.Close();
    }
}