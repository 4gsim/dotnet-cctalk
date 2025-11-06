namespace CcTalk.Coin;

/// <summary>
/// Represents a coin as recognized by a CcTalk coin acceptor.
/// </summary>
public struct CcTalkCoin
{
    /// <summary>
    /// The identifier string for the coin (e.g., currency code or type).
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The value of the coin as a floating-point number (e.g., 0.25 for 25 cents).
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// The integer representation of the coin's value (e.g., value in cents).
    /// </summary>
    public int IntValue { get; set; }

    /// <summary>
    /// Indicates whether the coin data is valid and recognized.
    /// </summary>
    public bool IsValid { get; set; }
}