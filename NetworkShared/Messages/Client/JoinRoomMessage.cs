using GameLogic.Enums;
using NetworkShared.Enums;
using System.Buffers.Binary;

namespace NetworkShared.Messages.Client;

public class JoinRoomMessage
{
    /// Encoded Message Structure:
    /// Byte 0: Message Length
    /// Byte 1: Message Code
    /// Bytes 2-5: Id of the room to join


    /// <summary>
    /// The number of bytes in the message (including the Length and Code bytes).
    /// </summary>
    public const int Length = 6;

    /// <summary>
    /// The ClientMessage message type.
    /// </summary>
    public const ClientMessage Code = ClientMessage.JoinRoom;

    
    public static int Decode(byte[] message)
    {
        byte[] roomIdBytes = new byte[4];
        Array.Copy(message, 2, roomIdBytes, 0, 4);
        int roomId = BinaryPrimitives.ReadInt32LittleEndian(roomIdBytes);

        return roomId;
    }


    public static byte[] Encode(int roomId)
    {
        byte codeByte = (byte)Code;
        byte[] roomIdBytes = BitConverter.GetBytes(roomId);

        byte[] message = new byte[Length];
        message[0] = Length;
        message[1] = codeByte;
        Array.Copy(roomIdBytes, 0, message, 2, roomIdBytes.Length);

        return message;
    }
}

