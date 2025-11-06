using System.Threading.Tasks;

namespace CcTalk;

/// <summary>
/// Represents a CcTalk command that returns a result of type <typeparamref name="TResult"/>.
/// </summary>
public interface ICcTalkCommand<TResult>
{
    /// <summary>
    /// Constant indicating a successful command execution.
    /// </summary>
    public const int Success = 1;

    /// <summary>
    /// Asynchronously executes the command and returns the result.
    /// </summary>
    /// <param name="source"></param>
    ///     The source of the device, 1 for host devices
    /// <param name="destination"></param>
    ///     The destination of the device, 0 - broadcast (single device on bus only)
    /// <param name="timeout">
    ///     The maximum time, in milliseconds, to wait for the first byte of a response.
    /// </param>
    /// <returns>
    /// A task that resolves to a tuple:
    /// - <see cref="CcTalkError"/>: An error if one occurred, otherwise null.
    /// - <typeparamref name="TResult"/>: The result of the command if successful, otherwise null.
    /// </returns>
    Task<(CcTalkError?, TResult?)> ExecuteAsync(byte source, byte destination, int timeout);

    /// <summary>
    /// Executes a command that expects only an acknowledgement (ACK/NACK) from the device.
    /// </summary>
    /// <param name="receiver">The CcTalk receiver to use for communication.</param>
    /// <param name="command">The command data block to send.</param>
    /// <param name="timeout">Timeout in milliseconds to wait for the first byte of a response. Default is 1000 ms.</param>
    /// <returns>
    /// A tuple containing a <see cref="CcTalkError"/> if an error occurred (otherwise null),
    /// and <see cref="Success"/> if an ACK was received, or null if not.
    /// </returns>
    static async Task<(CcTalkError?, object?)> ExecuteWithAckAsync(ICcTalkReceiver receiver, CcTalkDataBlock command,
        int timeout = 1000)
    {
        var (err, reply) = await receiver.ReceiveAsync(command, timeout);
        if (err != null)
        {
            return (err, null);
        }

        if (reply!.Value.Header == 5)
        {
            return (CcTalkError.FromMessage("Received NACK from device"), null);
        }

        return (null, Success);
    }
}