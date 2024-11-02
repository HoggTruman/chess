using Client;
using GameApplication.Windows.Game;
using GameLogic;
using GameLogic.Enums;
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

    private const string CommunicationErrorText = "An error occured while communicating with the server...";

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

        string stringCode = CodeTextBox.Text;
        if (IsValidCode(stringCode) == false)
        {
            JoinStatusTextBox.Text = InvalidCodeText;
            JoinButton.IsEnabled = true;
            CodeTextBox.IsEnabled = true;
            return;
        }

        JoinStatusTextBox.Text = AttemptingToJoinText;
        int code = int.Parse(stringCode);

        _gameClient = new GameClient();
        _gameClient.RoomNotFound += OnRoomNotFound;
        _gameClient.RoomFull += OnRoomFull;
        _gameClient.StartGame += OnStartGame;
        _gameClient.CommunicationError += OnCommunicationError;

        bool connected = await _gameClient.ConnectToServer();
        if (connected)
        {
            _ = _gameClient.StartListening();
            await _gameClient.SendJoinRoom(code);
        }
    }


    private void Back_Click(object sender, RoutedEventArgs e)
    {
        _gameClient?.Close();
        StartScreen startScreen = new(_window);
        _window.Content = startScreen;        
    }


    private void PasteButton_Click(object sender, RoutedEventArgs e)
    {
        string code = Clipboard.GetText();
        if (CodeTextBox.IsEnabled &&
            IsValidCode(code))
        {
            CodeTextBox.Text = code;
        }
    }

    #endregion



    #region GameManager Event Handlers

    private void OnRoomNotFound()
    {
        _gameClient?.Close();
        _gameClient = null;   

        JoinStatusTextBox.Text = RoomNotFoundText;
        JoinButton.IsEnabled = true;
        CodeTextBox.IsEnabled = true;
    }


    private void OnRoomFull()
    {
        _gameClient?.Close();
        _gameClient = null;
        

        JoinStatusTextBox.Text = RoomFullText;
        JoinButton.IsEnabled = true;
        CodeTextBox.IsEnabled = true;
    }


    private void OnStartGame(PieceColor playerColor)
    {
        if (_gameClient == null)
        {
            return;
        }

        _gameClient.RoomNotFound -= OnRoomNotFound;
        _gameClient.RoomFull -= OnRoomFull;
        _gameClient.StartGame -= OnStartGame;

        GameManager gameManager = new();
        GameWindow gameWindow = new(gameManager, playerColor, _gameClient);

        gameWindow.Show();
        _window.Close();
    }


    private void OnCommunicationError()
    {
        _gameClient?.Close();
        _gameClient = null;     

        JoinStatusTextBox.Text = CommunicationErrorText;
        JoinButton.IsEnabled = true;
        CodeTextBox.IsEnabled = true;
    }

    #endregion



    #region Other Methods

    private static bool IsValidCode(string code)
    {
        return code.Length >= 1 &&
               code.Length <= 9 &&
               code.All(char.IsDigit) &&
               code.StartsWith('0') == false;
    }

    #endregion

}

