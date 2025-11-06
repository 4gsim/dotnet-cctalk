namespace CcTalk.Bill;

public enum CcTalkBillRouteCode
{
    /// <summary>Return the bill from escrow position.</summary>
    Return = 0,
    /// <summary>Stack the bill from escrow position.</summary>
    Stack = 1,
    /// <summary>Extend escrow timeout.</summary>
    Hold = 255
}