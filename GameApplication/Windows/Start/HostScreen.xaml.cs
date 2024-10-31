using Client;
using GameApplication.Windows.Game;
using GameLogic;
using GameLogic.Enums;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameApplication.Windows.Start;

/// <summary>
/// Interaction logic for HostScreen.xaml
/// </summary>
public partial class HostScreen : UserControl
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
    private bool _colorButtonsEnabled = true;

    /// <summary>
    /// A brush to highlight the selected color.
    /// </summary>
    private static readonly SolidColorBrush HighlightBrush = new(Colors.LightGreen);

    #endregion



    #region Text

    private const string RoomNotHostedText = "(No Room Hosted)";

    private const string WaitingForOpponentText = "Waiting for opponent...";

    private const string ServerErrorText = "An error occured while communicating with the server...";

    #endregion



    #region Constructors

    public HostScreen(Window window)
    {
        InitializeComponent();
        _window = window;
    }

    #endregion



    #region Event Handlers

    private async void HostButton_Click(object sender, RoutedEventArgs e)
    {
        HostButton.IsEnabled = false;
        _colorButtonsEnabled = false;
        StatusTextBlock.Text = "";

        _gameClient = new GameClient();
        _gameClient.RoomHosted += OnRoomHosted;
        _gameClient.StartGame += OnStartGame;
        _gameClient.RoomClosed += OnRoomClosed;

        try
        {
            await _gameClient.ConnectToServer();
            _gameClient.StartListening();
            await _gameClient.SendHostRoom(_hostColor);
            
        }
        catch (Exception ex) when (
            ex is IOException || 
            ex is OperationCanceledException ||
            ex is SocketException)
        {
            StatusTextBlock.Text = ServerErrorText;
            _colorButtonsEnabled = true;
            await _gameClient.StopListening();
            _gameClient.Dispose();
            _gameClient = null;
            HostButton.IsEnabled = true;
        }
    }


    private async void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (_gameClient == null)
        {
            return;
        }

        CancelButton.IsEnabled = false;

        try
        {
            await _gameClient.SendCancelHost();
            _gameClient.CancellationTokenSource.Cancel();
        }
        catch (Exception ex) when (ex is IOException || ex is OperationCanceledException)
        {

        }
        finally
        {
            CancelButton.Visibility = Visibility.Hidden;
            HostButton.Visibility = Visibility.Visible;        

            StatusTextBlock.Text = "";
            RoomCodeTextBox.Text = RoomNotHostedText;

            _colorButtonsEnabled = true;
            await _gameClient.StopListening();
            _gameClient.Dispose();            
            _gameClient = null;
            CancelButton.IsEnabled = true;
        }
    }


    private void WhiteOption_Click(object sender, RoutedEventArgs e)
    {
        if (_colorButtonsEnabled)
        {
            _hostColor = PieceColor.White;
            WhiteOptionHighlight.Background = HighlightBrush;
            BlackOptionHighlight.Background = null;
        }
    }


    private void BlackOption_Click(object sender, RoutedEventArgs e)
    {
        if (_colorButtonsEnabled)
        {
            _hostColor = PieceColor.Black;
            BlackOptionHighlight.Background = HighlightBrush;
            WhiteOptionHighlight.Background = null;
        }
    }


    private async void Back_Click(object sender, RoutedEventArgs e)
    {
        if (_gameClient?.Connected == true)
        {
            try
            {
                _gameClient.SendCancelHost().Wait();
            }
            catch (Exception ex) when (ex is IOException || ex is OperationCanceledException)
            {
                Console.WriteLine(ex.Message);
            }

            await _gameClient.StopListening();
            _gameClient.Dispose();
        }
        
        StartScreen startScreen = new(_window);
        _window.Content = startScreen;
    }

    #endregion



    #region GameManager Event Handlers

    private void OnRoomHosted(int roomId)
    {
        if (_gameClient == null)
        {
            return;
        }

        HostButton.IsEnabled = true;
        HostButton.Visibility = Visibility.Hidden;
        CancelButton.Visibility = Visibility.Visible;

        StatusTextBlock.Text = WaitingForOpponentText;
        RoomCodeTextBox.Text = roomId.ToString();    
    }


    private void OnStartGame(PieceColor playerColor)
    {
        if (_gameClient == null)
        {
            return;
        }

        _gameClient.RoomHosted -= OnRoomHosted;
        _gameClient.StartGame -= OnStartGame;
        _gameClient.RoomClosed -= OnRoomClosed;

        GameManager gameManager = new();
        GameWindow gameWindow = new(gameManager, playerColor, _gameClient);

        gameWindow.Show();
        _window.Close();
    }


    private async void OnRoomClosed(PieceColor pieceColor)
    {
        if (_gameClient != null)
        {
            await _gameClient.StopListening();
            _gameClient.Dispose();
            _gameClient = null;   
        }

        StatusTextBlock.Text = ServerErrorText;
        RoomCodeTextBox.Text = RoomNotHostedText;

        _colorButtonsEnabled = true;
        HostButton.Visibility = Visibility.Visible;
        CancelButton.Visibility = Visibility.Hidden;
    }

    #endregion

}

