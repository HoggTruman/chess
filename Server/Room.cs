using GameLogic;
using GameLogic.Enums;

namespace Server;
    
public class Room
{
    #region fields

    private static int _roomCount;

    private GameManager _gameManager;

    private bool _isLocked;

    #endregion



    #region Properties

    public int Id { get; }

    public bool IsJoinable
    { 
        get
        {
            return Players.Count == 1 && _isLocked == false;
        }
    }

    public List<Client> Players { get; } = [];

    public PieceColor HostColor { get; private set; }

    #endregion




    public Room(PieceColor hostColor)
    {
        Id = GenerateRoomId();
        Board board = new();
        board.Initialize();
        _gameManager = new(board, PieceColor.None);
        

        HostColor = hostColor;
    }


    public void StartNewGame()
    {
        _gameManager.StartNewGame(PieceColor.None);
        _isLocked = true;
    }


    public Client GetOpponent(Client client)
    {
        foreach (Client player in Players)
        {
            if (client != player)
            {
                return player;
            }
        }

        throw new Exception("The Room does not contain an opponent");
    }


    private int GenerateRoomId()
    {
        return ++_roomCount;
    }
}

