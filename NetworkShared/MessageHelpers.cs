using NetworkShared.Enums;

namespace NetworkShared;

public class MessageHelpers
{
    /// <summary>
    /// Gets the ServerMessage code from a byte array message.
    /// </summary>
    /// <param name="message">A byte array representing the message.</param>
    /// <returns>A ServerMessage enum</returns>
    public static ServerMessage ReadServerCode(byte[] message)
    {
        return (ServerMessage)message[0];
    }

    /// <summary>
    /// Gets the ClientMessage code from a byte array message.
    /// </summary>
    /// <param name="message">A List of bytes representing the message.</param>
    /// <returns>A ClientMessage enum</returns>
    public static ClientMessage ReadClientCode(byte[] message)
    {
        return (ClientMessage)message[0];
    }
}
