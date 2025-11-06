namespace CcTalk.Bill;

public enum CcTalkBillPosition
{
    /// <summary>Position unknown.</summary>
    Unknown = -1,
    /// <summary>Bill was sent to the cash box.</summary>
    Stacked = 0,
    /// <summary>Holding bill in escrow position.</summary>
    Escrow = 1,
    /// <summary>For adp AFD-MONO: bill moved to dispenser SS1.</summary>
    AFD_DispenserSS1 = 18,
    /// <summary>For adp AFD-MONO: bill moved to dispenser SS2.</summary>
    AFD_DispenserSS2 = 19,
    /// <summary>For adp AFD-MONO: bill moved to dispenser SS3.</summary>
    AFD_DispenserSS3 = 20,
    /// <summary>Bill form recycler stored in cash box.</summary>
    AFD_Stored = 32,
}