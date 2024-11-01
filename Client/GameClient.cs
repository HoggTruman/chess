using GameLogic.Enums;
using GameLogic.Interfaces;
using NetworkShared;
using NetworkShared.Enums;
using NetworkShared.Messages.Client;
using NetworkShared.Messages.Server;
using System.Net;
using System.Net.Sockets;

namespace Client;

/// <summary>
/// A class which handles server communication.
/// A new GameClient should be created on hosting a room or on
/// attempting to join a room.
/// </summary>
public class GameClient : IDisposable
{
    #region Fields

    private readonly TcpClient _tcpClient;
    private NetworkStream? _stream;
    private readonly byte[] _buffer = new byte[32];
    private bool _isDisposed = false;
    private Task? _listeningTask;

    #endregion



    #region Properties

    public CancellationTokenSource CancellationTokenSource { get; } = new();
    
    public CancellationToken Token { get; }

    public bool Connected
    {
        get => _stream != null && _tcpClient.Connected && _isDisposed == false;
    }

    #endregion



    #region Events

    public event Action<int>? RoomHosted;

    public event Action<PieceColor>? StartGame;

    public event Action? RoomNotFound;

    public event Action? RoomFull;

    public event Action<PieceColor>? RoomClosed;

    public event Action<IMove>? MoveReceived;

    public event Action? CommunicationError;

    #endregion



    public GameClient()
    {
        _tcpClient = new TcpClient();
        Token = CancellationTokenSource.Token;
    }


    /// <summary>
    /// Connects the GameClient to the server.
    /// This method must be called before attempting to interact with the server.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ConnectToServer()
    {
        if (Connected)
        {
            throw new InvalidOperationException("Already connected to server.");
        }

        var ipEndpoint = new IPEndPoint(IPAddress.Parse(ServerInfo.IpAddress), ServerInfo.Port);
        try
        {
            await _tcpClient.ConnectAsync(ipEndpoint, Token);
        }
        catch (SocketException)
        {
            CommunicationError?.Invoke();
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }        

        if (_tcpClient.Connected)
        {
            _stream = _tcpClient.GetStream();
            _listeningTask = Listen();
            return true;
        }

        return false;
    }


    private async Task Listen()
    {
        try
        {
            while (_stream != null &&
                   Token.IsCancellationRequested == false)
            {
                var message = await ReadServerMessage();
                if (message.Length == 0)
                {
                    RoomClosed?.Invoke(PieceColor.None);
                    return;
                }
                HandleServerMessage(message);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (IOException)
        {
            CommunicationError?.Invoke();
        }
    }


    public async Task StopListening()
    {
        CancellationTokenSource.Cancel();

        if (_listeningTask != null)
        {            
            await _listeningTask;
            _listeningTask = null;
        }

        // handle disposal here, make this a "Close" method??
    }


    /// <summary>
    /// Reads a message from the server.
    /// </summary>
    /// <returns>
    /// A Task that represents the async read operation.
    /// The value of its result is the message read as a byte array.
    /// </returns>
    /// <exception cref="IOException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public async Task<byte[]> ReadServerMessage()
    {
        if (_stream == null)
        {
            throw new InvalidOperationException("No NetworkSteam to write to. Call ConnectToServer first.");
        }

        // get message length
        int bytesRead = await _stream.ReadAsync(_buffer, 0, 1, Token);        

        if (bytesRead == 0)
        {
            return [];
        }
        
        // read message
        byte messageLength = _buffer[0];
        byte[] message = new byte[messageLength];
        message[0] = messageLength;
        await _stream.ReadAsync(message, 1, messageLength - 1, Token);

        return message;
    }


    /// <summary>
    /// Decodes a message from the server and invokes a corresponding event.
    /// </summary>
    /// <param name="message"></param>
    public void HandleServerMessage(byte[] message)
    {
        ServerMessage msgCode = MessageHelpers.ReadServerCode(message);

        switch (msgCode)
        {
            case ServerMessage.RoomHosted:
                int roomId = RoomHostedMessage.Decode(message);
                RoomHosted?.Invoke(roomId);
                break;

            case ServerMessage.StartGame:
                PieceColor playerColor = StartGameMessage.Decode(message);
                StartGame?.Invoke(playerColor);
                break;

            case ServerMessage.RoomNotFound:
                RoomNotFound?.Invoke();
                break;

            case ServerMessage.RoomFull:
                RoomFull?.Invoke();
                break;

            case ServerMessage.RoomClosed:
                PieceColor winnerColor = RoomClosedMessage.Decode(message);
                RoomClosed?.Invoke(winnerColor);
                break;

            case ServerMessage.Move:
                IMove move = ServerMoveMessage.Decode(message);
                MoveReceived?.Invoke(move);
                break;
        }
    }



    #region Send Message To Server Methods

    /// <summary>
    /// Sends a message to the server to host a room.
    /// </summary>
    /// <param name="hostColor">The PieceColor of the host.</param>
    /// <returns></returns>
    public async Task SendHostRoom(PieceColor hostColor)
    {
        byte[] message = HostRoomMessage.Encode(hostColor);
        await SendMessage(message);
    }


    /// <summary>
    /// Sends a message to the server to join a room.
    /// </summary>
    /// <param name="roomId">The ID of the room to join.</param>
    /// <returns></returns>
    public async Task SendJoinRoom(int roomId)
    {
        byte[] message = JoinRoomMessage.Encode(roomId);
        await SendMessage(message);
    }


    /// <summary>
    /// Sends a message to the server to cancel hosting a room.
    /// </summary>
    /// <returns></returns>
    public async Task SendCancelHost()
    {
        byte[] message = CancelHostMessage.Encode();
        await SendMessage(message);
    }


    /// <summary>
    /// Sends a message to the server containing the player's IMove.
    /// </summary>
    /// <param name="move">The IMove the player is making.</param>
    /// <returns></returns>
    public async Task SendMove(IMove move)
    {        
        byte[] message = ClientMoveMessage.Encode(move);
        await SendMessage(message);
    }


    private async Task SendMessage(byte[] message)
    {
        if (_stream == null)
        {
            throw new InvalidOperationException("No NetworkSteam to write to. Call ConnectToServer first.");
        }

        try
        {
            await _stream.WriteAsync(message, Token);
        }
        catch (IOException)
        {
            CommunicationError?.Invoke();      
        }
        catch (OperationCanceledException)
        {

        }
    }

    #endregion



    public void Dispose()
    {
        if (_isDisposed == false)
        {
            _isDisposed = true;
            _tcpClient.Close();
            CancellationTokenSource.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

