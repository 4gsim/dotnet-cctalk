namespace CcTalk;

/// <summary>
/// Specifies the type of checksum used in CcTalk communication.
/// </summary>
public enum CcTalkChecksumType
{
    /// <summary>
    /// 8-bit simple checksum (sum of all bytes should be zero).
    /// </summary>
    Simple8,

    /// <summary>
    /// 16-bit CRC checksum (used for extended CcTalk protocol).
    /// </summary>
    Crc16
}