namespace CcTalk;

/// <summary>
/// Represents a CcTalk protocol data block, containing a header and associated data bytes.
/// </summary>
public readonly struct CcTalkDataBlock
{
    /// <summary>
    /// The source byte of the CcTalk message, indicating the source device.
    /// </summary>
    public byte Source { get; init; }

    /// <summary>
    /// The destination byte of the CcTalk message, indicating destination device.
    /// </summary>
    public byte Destination { get; init; }

    /// <summary>
    /// The header byte of the CcTalk message, indicating the command or response type.
    /// </summary>
    public byte Header { get; init; }

    /// <summary>
    /// The data payload of the CcTalk message.
    /// </summary>
    public byte[] Data { get; init; } = [];

    /// <summary>
    /// The length of the data payload.
    /// </summary>
    public byte DataLength => (byte)Data.Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="CcTalkDataBlock"/> struct.
    /// </summary>
    public CcTalkDataBlock()
    {
        // No initialization required; properties use default values.
    }
}