using GameLogic;
using GameLogic.Enums;

namespace Server;
    
public class Room
{
    private static int _roomCount;

    private GameManager _gameManager;
    

    public int Id { get; }

    public bool IsFull 
    { 
        get
        {
            return Players.Count == 2;
        }
    }

    public List<Client> Players { get; } = [];

    public PieceColor HostColor { get; private set; }

    

    public Room(PieceColor hostColor)
    {
        Id = GenerateRoomId();
        _gameManager = new(new Board(), PieceColor.None);
        

        HostColor = hostColor;
    }



    private int GenerateRoomId()
    {
        return ++_roomCount;
    }
}

