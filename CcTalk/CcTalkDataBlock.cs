namespace CcTalk;

public struct CcTalkDataBlock
{
    public byte Header { get; set; }
    public byte[] Data { get; set; } = [];
    public readonly byte DataLength => (byte)Data.Length;

    public CcTalkDataBlock()
    {
        
    }
}