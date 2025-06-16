using System;
using NUnit.Framework;

namespace CcTalk.Tests;

public class CcTalkCrc16Tests
{
    private static readonly ushort[] CrcTable;

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

    static CcTalkCrc16Tests()
    {
        CrcTable = BuildCrcTable();
    }

    [TestCase(new byte[] { 0x28, 0x00, 0x00, 0x01, 0x00 }, 0x3f, 0x46)]
    [TestCase(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00 }, 0x37, 0x30)]
    public void CalculateCrc16(byte[] bytes, byte expectedMsb, byte expectedLsb)
    {
        ushort crc = 0;
        for (var i = 0; i < bytes.Length - 1; ++i)
        {
            if (i == 2)
            {
                continue;
            }
            crc = (ushort)((ushort)(crc << 8) ^ CrcTable[(byte)((crc >> 8) ^ bytes[i])]);
        }
        Assert.That((byte)crc, Is.EqualTo(expectedLsb));
        Assert.That((byte)(crc >> 8), Is.EqualTo(expectedMsb));
    }
}