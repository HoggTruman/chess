using GameLogic;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Moves;
using Server.Interfaces;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Server;

public class Room
{
    #region fields

    private readonly GameManager _gameManager;

    private readonly static Random _random = new();
    private readonly static ConcurrentDictionary<int, byte> _activeIds = [];
    
    private readonly List<IClient> _players = [];
    private readonly Dictionary<IClient, PieceColor> _playerColors = [];

    private bool _isLocked;

    #endregion



    #region Constants

    private const int MinRoomId = 100000000;
    private const int MaxRoomId = 999999999;

    #endregion



    #region Properties

    public int Id { get; }

    public ReadOnlyCollection<IClient> Players
    {
        get => _players.AsReadOnly();
    }

    public ReadOnlyDictionary<IClient, PieceColor> PlayerColors
    {
        get => _playerColors.AsReadOnly();
    }

    public IClient Host
    {
        get => _players[0];
    }

    #endregion



    #region Constructors

    public Room(IClient hostClient, PieceColor hostColor)
    {
        Id = GenerateRoomId();
        _gameManager = new GameManager();
        
        _players.Add(hostClient);
        _playerColors[hostClient] = hostColor;
    }

    #endregion

    

    #region Public Methods

    /// <summary>
    /// Attempts to add a Client to the room.
    /// </summary>
    /// <param name="joiningClient"></param>
    /// <returns>true if successful. Otherwise, false</returns>
    public bool TryJoin(IClient joiningClient)
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
    public IClient GetOpponent(IClient client)
    {
        foreach (IClient player in Players)
        {
            if (client != player)
            {
                return player;
            }
        }

        throw new Exception("The Room does not contain an opponent");
    }


    /// <summary>
    /// Gets the PieceColor of the client's opponent.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public PieceColor GetOpponentColor(IClient client)
    {
        if (_playerColors.TryGetValue(client, out PieceColor playerColor))
        {
            return ColorHelpers.Opposite(playerColor);
        }

        throw new ArgumentException("Client is not a member of this room.");
    }


    /// <summary>
    /// Performs server-side validation of the IMove and applies it if it is valid.
    /// </summary>
    /// <param name="client">The Client object of the player attempting to move.</param>
    /// <param name="playerMove">The IMove to attempt.</param>
    /// <returns>true if the move is valid. Otherwise, false.</returns>
    public bool TryMove(IClient client, IMove playerMove)
    {
        if (_gameManager.ActivePlayerColor != _playerColors[client] ||
            _gameManager.IsValidMove(playerMove) == false)
        {
            return false;
        }

        _gameManager.HandleMove(playerMove);
        _gameManager.SwitchTurn();
        return true;
    }


    /// <summary>
    /// Determines if the game is over.
    /// </summary>
    /// <returns>true if the game is over. Otherwise, false.</returns>
    public bool GameIsOver()
    {
        return _gameManager.GameIsOver();
    }


    /// <summary>
    /// Determines the winner of the game.
    /// </summary>
    /// <returns>A PieceColor</returns>
    public PieceColor GetWinner()
    {
        var (winnerColor, _) = _gameManager.GetGameResult();
        return winnerColor;
    }


    /// <summary>
    /// Allows the room's Id to be reused.
    /// </summary>
    public void Close()
    {
        _activeIds.Remove(Id, out _);
    }

    #endregion



    #region Private Methods

    private static int GenerateRoomId()
    {
        if (_activeIds.Count == MaxRoomId - MinRoomId + 1)
        {
            throw new Exception("Room capacity reached.");
        }

        int id = _random.Next(MinRoomId, MaxRoomId);
        while (_activeIds.ContainsKey(id))
        {
            id++;

            if (id > MaxRoomId)
            {
                id = MinRoomId;
            }
        }

        _activeIds[id] = 0;

        return id;
    }

    #endregion

}

