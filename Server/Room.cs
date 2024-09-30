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
            return Players[PieceColor.White] != null && Players[PieceColor.Black] != null;
        }
    }

    public Dictionary<PieceColor, Client?> Players { get; }

    

    public Room()
    {
        Id = GenerateRoomId();
        _gameManager = new(new Board(), PieceColor.None);
        Players = new Dictionary<PieceColor, Client?>
        {
            [PieceColor.White] = null,
            [PieceColor.Black] = null
        };
    }


    private int GenerateRoomId()
    {
        return ++_roomCount;
    }
}

