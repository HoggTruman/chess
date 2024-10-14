using GameLogic;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using System.Collections.ObjectModel;

namespace Server;
    
public class Room
{
    #region fields

    private static int _roomCount;

    private GameManager _gameManager;

    private List<Client> _players = [];

    private Dictionary<Client, PieceColor> _playerColors = [];

    private bool _isLocked;

    #endregion



    #region Properties

    public int Id { get; }

    public ReadOnlyCollection<Client> Players
    {
        get => _players.AsReadOnly();
    }

    public ReadOnlyDictionary<Client, PieceColor> PlayerColors
    {
        get => _playerColors.AsReadOnly();
    }

    public Client Host
    {
        get => _players[0];
    }

    public bool GameIsOver { get; private set; }

    #endregion




    public Room(Client hostClient, PieceColor hostColor)
    {
        Id = GenerateRoomId();
        Board board = new();
        board.Initialize();
        _gameManager = new(board, PieceColor.None);
        
        _players.Add(hostClient);
        _playerColors[hostClient] = hostColor;
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

        PieceColor playerColor = ColorHelpers.Opposite(_playerColors[Host]);
        
        _players.Add(joiningClient);
        _playerColors[joiningClient] = playerColor;
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


    /// <summary>
    /// Performs server-side validation of the IMove and applies it if it is valid.
    /// </summary>
    /// <param name="client">The Client object of the player attempting to move.</param>
    /// <param name="playerMove">The IMove to attempt.</param>
    /// <returns>true if the move is valid. Otherwise, false.</returns>
    public bool TryMove(Client client, IMove playerMove)
    {
        if (_gameManager.ActivePlayerColor != _playerColors[client])
        {
            // Return false if it is not the client's turn to move.
            return false;
        }

        var validMoves = _gameManager.ActivePlayerMoves[playerMove.From.row, playerMove.From.col];

        if (validMoves == null)
        {
            // Return false if the from square does not contain one of the client's pieces. 
            return false;
        }

        foreach (IMove move in validMoves)
        {
            if (playerMove.IsEquivalentTo(move))
            {
                _gameManager.HandleMove(move);
                _gameManager.SwitchTurn();
                GameIsOver = _gameManager.GameIsOver();
                return true;
            }
        }

        return false;
    }


    private int GenerateRoomId()
    {
        return ++_roomCount;
    }
}

