using System;

namespace CcTalk;

/// <summary>
/// Represents an error that occurred during a CcTalk operation.
/// </summary>
public readonly struct CcTalkError
{
    /// <summary>
    /// The error message describing the error.
    /// </summary>
    public string Messaage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CcTalkError"/> struct with a specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    private CcTalkError(string message)
    {
        Messaage = message;
    }

    /// <summary>
    /// Creates a <see cref="CcTalkError"/> from a custom message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new <see cref="CcTalkError"/> instance.</returns>
    public static CcTalkError FromMessage(string message)
    {
        return new CcTalkError(message);
    }

    /// <summary>
    /// Creates a <see cref="CcTalkError"/> from an exception.
    /// </summary>
    /// <param name="e">The exception to extract the message from.</param>
    /// <returns>A new <see cref="CcTalkError"/> instance.</returns>
    public static CcTalkError FromException(Exception e)
    {
        return new CcTalkError(e.Message);
    }

    /// <summary>
    /// Returns the error message as a string.
    /// </summary>
    /// <returns>The error message.</returns>
    public override string ToString()
    {
        return Messaage;
    }
}