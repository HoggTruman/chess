using NetworkShared.Enums;
using System.Buffers.Binary;

namespace NetworkShared.Messages.Server;

public class RoomHostedMessage
{
    // Encoded Message Structure:
    // Byte 0: Message code
    // Bytes 1-4: roomId


    public static ServerMessage Code { get; } = ServerMessage.RoomHosted;


    public static int Decode(byte[] message)
    {
        byte[] roomIdBytes = new byte[4];
        Array.Copy(message, 1, roomIdBytes, 0, 4);
        int roomId = BinaryPrimitives.ReadInt32LittleEndian(roomIdBytes);

        return roomId;
    }


    public static byte[] Encode(int roomId)
    {
        byte codeByte = (byte)ServerMessage.RoomHosted;
        byte[] roomIdBytes = BitConverter.GetBytes(roomId);

        byte[] message = new byte[5];
        message[0] = codeByte;
        Array.Copy(roomIdBytes, 0, message, 1, roomIdBytes.Length);

        return message;
    }
}

