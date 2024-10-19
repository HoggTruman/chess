using Client;
using GameApplication.Windows.Game;
using GameLogic;
using GameLogic.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameApplication.Windows.Start;

/// <summary>
/// Interaction logic for HostScreen.xaml
/// </summary>
public partial class HostScreen : UserControl, IDisposable
{
    #region Fields

    /// <summary>
    /// The Window object this HostScreen is contained within.
    /// </summary>
    private readonly Window _window;

    /// <summary>
    /// The GameClient used to communicate with the server.
    /// </summary>
    private GameClient? _gameClient;

    /// <summary>
    /// The selected PieceColor of the host.
    /// </summary>
    private PieceColor _hostColor = PieceColor.White;

    /// <summary>
    /// A bool to control what elements are rendered based on if the room has been hosted.
    /// </summary>
    private bool _waitingForOpponent = false;

    /// <summary>
    /// A brush to highlight the selected color.
    /// </summary>
    private static readonly SolidColorBrush HighlightBrush = new(Colors.LightGreen);

    #endregion



    #region Constructors

    public HostScreen(Window window)
    {
        InitializeComponent();
        _window = window;
    }

    #endregion



    #region Event Handlers

    private async void HostButton_ClickHost(object sender, RoutedEventArgs e)
    {
        HostButton.Content = "Cancel Host";
        HostButton.Click -= HostButton_ClickHost;
        HostButton.Click += HostButton_ClickCancel;

        _waitingForOpponent = true;

        _gameClient = new GameClient();
        _gameClient.RoomHosted += OnRoomHosted;
        _gameClient.StartGame += OnStartGame;

        try
        {
            await _gameClient.ConnectToServer();
            await _gameClient.SendHostRoom(_hostColor);
            await _gameClient.ReadAndHandleServerMessage();
        }
        catch(Exception)
        {
            StatusTextBlock.Text = "An error occured while communicating with the server...";
            _gameClient = null;
        }

    }


    private void HostButton_ClickCancel(object sender, RoutedEventArgs e)
    {
        HostButton.Content = "Host Game";
        HostButton.Click -= HostButton_ClickCancel;
        HostButton.Click += HostButton_ClickHost;

        _waitingForOpponent = false;
        _gameClient = null;
    }


    private void WhiteOption_Click(object sender, RoutedEventArgs e)
    {
        if (_waitingForOpponent == false)
        {
            _hostColor = PieceColor.White;
            WhiteOptionHighlight.Background = HighlightBrush;
            BlackOptionHighlight.Background = null;
        }

    }


    private void BlackOption_Click(object sender, RoutedEventArgs e)
    {
        if (_waitingForOpponent == false)
        {
            _hostColor = PieceColor.Black;
            BlackOptionHighlight.Background = HighlightBrush;
            WhiteOptionHighlight.Background = null;
        }
    }


    private void Back_Click(object sender, RoutedEventArgs e)
    {
        StartScreen startScreen = new(_window);
        _window.Content = startScreen;
    }

    #endregion



    #region GameManager Event Handlers

    private async void OnRoomHosted(int roomId)
    {
        StatusTextBlock.Text = "Waiting for opponent...";
        RoomCodeTextBlock.Text = roomId.ToString();
        await _gameClient.ReadAndHandleServerMessage();
    }


    private void OnStartGame(PieceColor playerColor)
    {
        Dispose();
        GameManager gameManager = new(new Board());
        GameWindow gameWindow = new(gameManager, playerColor, _gameClient);

        gameWindow.Show();
        _window.Close();
    }

    #endregion




    public void Dispose()
    {
        if (_gameClient != null)
        {
            _gameClient.RoomHosted -= OnRoomHosted;
            _gameClient.StartGame -= OnStartGame;
        }
    }
}

