namespace NetworkShared;

public class MessageHelpers
{
    /// <summary>
    /// Returns the message code as a byte.
    /// </summary>
    /// <param name="message">A List of bytes representing the message.</param>
    /// <returns></returns>
    public static byte ReadCode(List<byte> message)
    {
        return message[0];
    }
}

