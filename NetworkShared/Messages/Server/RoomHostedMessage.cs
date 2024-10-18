using NetworkShared.Enums;
using System.Buffers.Binary;

namespace NetworkShared.Messages.Server;

public class RoomHostedMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message Length
    /// Byte 1: Message Code
    /// Bytes 2-5: roomId


    /// <summary>
    /// The number of bytes in the message (including the Length and Code bytes).
    /// </summary>
    public const int Length = 6;

    /// <summary>
    /// The ServerMessage message type.
    /// </summary>
    public const ServerMessage Code = ServerMessage.RoomHosted;


    /// <summary>
    /// Decodes a room ID from a RoomHostedMessage byte array.
    /// </summary>
    /// <param name="message">A RoomHostedMessage byte array.</param>
    /// <returns>An int representing the room ID.</returns>
    public static int Decode(byte[] message)
    {
        byte[] roomIdBytes = new byte[4];
        Array.Copy(message, 2, roomIdBytes, 0, 4);
        int roomId = BinaryPrimitives.ReadInt32LittleEndian(roomIdBytes);

        return roomId;
    }


    /// <summary>
    /// Encodes a room ID to a RoomHostedMessage byte array.
    /// </summary>
    /// <param name="roomId">An int representing the room ID.</param>
    /// <returns>A RoomHostedMessage byte array.</returns>
    public static byte[] Encode(int roomId)
    {
        byte codeByte = (byte)ServerMessage.RoomHosted;
        byte[] roomIdBytes = BitConverter.GetBytes(roomId);

        byte[] message = new byte[Length];
        message[0] = Length;
        message[1] = codeByte;
        Array.Copy(roomIdBytes, 0, message, 2, 4);

        return message;
    }
}

