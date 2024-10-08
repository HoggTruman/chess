using GameLogic;
using GameLogic.Enums;
using System.Collections.ObjectModel;

namespace Server;
    
public class Room
{
    #region fields

    private static int _roomCount;

    private GameManager _gameManager;

    private List<Client> _players = [];

    private bool _isLocked;

    #endregion



    #region Properties

    public int Id { get; }

    public ReadOnlyCollection<Client> Players
    {
        get => _players.AsReadOnly();
    }

    public PieceColor HostColor { get; private set; }

    #endregion




    public Room(Client hostClient, PieceColor hostColor)
    {
        Id = GenerateRoomId();
        Board board = new();
        board.Initialize();
        _gameManager = new(board, PieceColor.None);
        
        _players.Add(hostClient);
        HostColor = hostColor;
    }


    public void StartNewGame()
    {
        _gameManager.StartNewGame(PieceColor.None);
    }


    /// <summary>
    /// Attempts to add a Client to the room.
    /// </summary>
    /// <param name="joiningClient"></param>
    /// <returns>true if successful. Otherwise, false</returns>
    public bool TryJoin(Client joiningClient)
    {
        if (_isLocked || Players.Count != 1)
        {
            return false;
        }
        
        _players.Add(joiningClient);
        _isLocked = true;

        return true;
    }


    /// <summary>
    /// Gets the Client object of the opponent.
    /// </summary>
    /// <param name="client">The Client object of the player</param>
    /// <returns></returns>
    /// <exception cref="Exception">The Room does not contain an opponent</exception>
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

