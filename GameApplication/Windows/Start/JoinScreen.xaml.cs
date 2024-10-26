using Client;
using GameApplication.Windows.Game;
using GameLogic;
using GameLogic.Enums;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Start;

/// <summary>
/// Interaction logic for JoinScreen.xaml
/// </summary>
public partial class JoinScreen : UserControl
{
    #region Fields

    /// <summary>
    /// The Window object this JoinScreen is contained within.
    /// </summary>
    private readonly Window _window;

    /// <summary>
    /// The GameClient used to communicate with the server.
    /// </summary>
    private GameClient? _gameClient;

    #endregion



    #region Text

    private const string InvalidCodeText = "Invalid Code Provided";

    private const string RoomNotFoundText = "A room can not be found with the provided code";

    private const string RoomFullText = "The room you are trying to join is full";

    private const string AttemptingToJoinText = "Attempting to join room...";

    private const string ServerErrorText = "An error occured while communicating with the server...";

    #endregion



    #region Constructors

    public JoinScreen(Window window)
    {
        InitializeComponent();
        _window = window;
    }

    #endregion



    #region Event Handlers

    private async void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        JoinButton.IsEnabled = false;
        CodeTextBox.IsEnabled = false;

        _gameClient = new GameClient();
        _gameClient.RoomNotFound += OnRoomNotFound;
        _gameClient.RoomFull += OnRoomFull;
        _gameClient.StartGame += OnStartGame;

        string stringCode = CodeTextBox.Text;
        if (ValidateCode(stringCode) == false)
        {
            JoinStatusTextBox.Text = InvalidCodeText;
            JoinButton.IsEnabled = true;
            CodeTextBox.IsEnabled = true;
            return;
        }

        JoinStatusTextBox.Text = AttemptingToJoinText;
        int code = int.Parse(stringCode);

        try
        {
            await _gameClient.ConnectToServer();
            await _gameClient.SendJoinRoom(code);
            var message = await _gameClient.ReadServerMessage();
            _gameClient.HandleServerMessage(message);
        }
        catch (Exception ex) when (
            ex is IOException || 
            ex is OperationCanceledException ||
            ex is SocketException)
        {
            JoinStatusTextBox.Text = ServerErrorText;
            JoinButton.IsEnabled = true;
            CodeTextBox.IsEnabled = true;
            _gameClient?.Dispose();
            _gameClient = null;
        }        
    }


    private void Back_Click(object sender, RoutedEventArgs e)
    {
        _gameClient?.CancellationTokenSource.Cancel();
        StartScreen startScreen = new(_window);
        _window.Content = startScreen;
        _gameClient?.Dispose(); // Could cause ObjectDisposedException  for the token in JoinGame_Click
    }

    #endregion



    #region GameManager Event Handlers

    private void OnRoomNotFound()
    {
        JoinStatusTextBox.Text = RoomNotFoundText;
        JoinButton.IsEnabled = true;
        CodeTextBox.IsEnabled = true;
        _gameClient?.Dispose();
        _gameClient = null;
    }


    private void OnRoomFull()
    {
        JoinStatusTextBox.Text = RoomFullText;
        JoinButton.IsEnabled = true;
        CodeTextBox.IsEnabled = true;
        _gameClient?.Dispose();
        _gameClient = null;
    }


    private void OnStartGame(PieceColor playerColor)
    {
        _gameClient!.RoomNotFound -= OnRoomNotFound;
        _gameClient.RoomFull -= OnRoomFull;
        _gameClient.StartGame -= OnStartGame;

        GameManager gameManager = new();
        GameWindow gameWindow = new(gameManager, playerColor, _gameClient);

        gameWindow.Show();
        _window.Close();
    }

    #endregion



    #region Other Methods

    private static bool ValidateCode(string code)
    {
        if (code.Length < 1 ||
            code.Length > 9 ||
            code.All(char.IsDigit) == false ||
            code.StartsWith('0'))
        {
            return false;
        }

        return true;
    }

    #endregion
}

