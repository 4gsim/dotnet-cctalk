using System;
using System.Threading.Tasks;

namespace CcTalk;

/// <summary>
/// Represents a receiver capable of sending CcTalk commands and receiving responses.
/// </summary>
public interface ICcTalkReceiver : IDisposable
{
    /// <summary>
    /// Sends a CcTalk command and asynchronously waits for a response.
    /// </summary>
    /// <param name="command">The CcTalk data block to send to the device.</param>
    /// <param name="timeout">The maximum time, in milliseconds, to wait for the first byte of a response.</param>
    /// <returns>
    /// A task that resolves to a tuple:
    /// - <see cref="CcTalkError"/>: An error if one occurred, otherwise null.
    /// - <see cref="CcTalkDataBlock"/>: The response from the device if successful, otherwise null.
    /// </returns>
    Task<(CcTalkError?, CcTalkDataBlock?)> ReceiveAsync(CcTalkDataBlock command, int timeout);
}