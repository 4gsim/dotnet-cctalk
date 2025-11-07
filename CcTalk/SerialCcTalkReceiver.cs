using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("CcTalk.Tests")]

namespace CcTalk;

internal class SerialCcTalkReceiver(
    ISerialReceiver serialReceiver,
    CcTalkChecksumType checksumType = CcTalkChecksumType.Simple8) : ICcTalkReceiver
{
    private static readonly ushort[] CrcTable;

    static SerialCcTalkReceiver()
    {
        CrcTable = BuildCrcTable();
    }

    private byte[] BuildCommand(CcTalkDataBlock command)
    {
        var messageLength = 5 + command.Data.Length;
        var bytes = new byte[messageLength];
        bytes[0] = command.Destination;
        bytes[1] = command.DataLength;
        bytes[2] = command.Source;
        bytes[3] = command.Header;
        var dataSum = (byte)0;
        for (var i = 0; i < command.DataLength; i++)
        {
            bytes[4 + i] = command.Data[i];
            if (checksumType == CcTalkChecksumType.Simple8)
            {
                dataSum = (byte)(dataSum + command.Data[i]);
            }
        }

        if (checksumType == CcTalkChecksumType.Simple8)
        {
            bytes[messageLength - 1] =
                (byte)(256 - (byte)(command.Source + command.Destination + command.Header + command.DataLength +
                                    dataSum));
        }
        else
        {
            var crc = CalculateCrc16Checksum(bytes);
            bytes[2] = (byte)crc;
            bytes[messageLength - 1] = (byte)(crc >> 8);
        }

        return bytes;
    }

    private (CcTalkError?, CcTalkDataBlock?) Deserialize(byte[] bytes)
    {
        try
        {
            if (bytes[0] != 1)
            {
                return (CcTalkError.FromMessage($"Received wrong destination {bytes[0]}"), null);
            }

            var data = new CcTalkDataBlock
            {
                Header = bytes[3],
                Data = new byte[bytes[1]]
            };
            if (checksumType == CcTalkChecksumType.Simple8)
            {
                var sum = (byte)(bytes[0] + bytes[1] + bytes[2] + bytes[3] + bytes[4 + bytes[1]]);
                for (var i = 0; i < bytes[1]; i++)
                {
                    data.Data[i] = bytes[4 + i];
                    sum = (byte)(sum + bytes[4 + i]);
                }

                if (sum != 0)
                {
                    return (CcTalkError.FromMessage("Incorrect checksum"), null);
                }
            }
            else
            {
                var crc = CalculateCrc16Checksum(bytes);
                if (bytes[2] != (byte)crc || bytes[^1] != (byte)(crc >> 8))
                {
                    return (CcTalkError.FromMessage("Incorrect checksum"), null);
                }
            }

            return (null, data);
        }
        catch (Exception e)
        {
            return (CcTalkError.FromException(e), null);
        }
    }

    public async Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, int timeout)
    {
        var request = BuildCommand(command);
        var response = await serialReceiver.ReceiveAsync(request, timeout);
        return Deserialize(response);
    }

    private static ushort[] BuildCrcTable()
    {
        var table = new ushort[256];
        for (var i = 0; i < 256; ++i)
        {
            var crc = i << 8 ^ 0;
            for (var j = 0; j < 8; ++j)
            {
                if ((crc & 0x8000) == 0x8000)
                {
                    crc = (crc << 1) ^ 0x1021; // 0001.0000 0010.0001 = x^12 + x^5 + 1 ( + x^16 )
                }
                else
                {
                    crc <<= 1;
                }
            }

            table[i] = (ushort)crc;
        }

        return table;
    }

    private static ushort CalculateCrc16Checksum(byte[] bytes)
    {
        ushort crc = 0;
        for (var i = 0; i < bytes.Length - 1; ++i)
        {
            if (i == 2)
            {
                continue; // Skip the least significant byte of the checksum
            }

            crc = (ushort)((ushort)(crc << 8) ^ CrcTable[(byte)((crc >> 8) ^ bytes[i])]);
        }

        return crc;
    }

    public void Dispose()
    {
    }
}