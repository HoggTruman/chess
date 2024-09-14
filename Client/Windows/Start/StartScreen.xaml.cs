using Client.Windows.Game;
using GameLogic;
using GameLogic.Enums;
using System.Windows;
using System.Windows.Controls;

namespace Client.Windows.Start;

/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
public partial class StartScreen : UserControl
{
    private readonly Window window;

    public StartScreen(Window window)
    {
        InitializeComponent();
        this.window = window;
    }


    private void StartGame(PieceColor playerColor)
    {
        Board board = new();
        board.Initialize();
        GameManager gameManager = new(board, playerColor); 

        GameWindow gameWindow = new(gameManager);
        gameWindow.Show();

        window.Close();
    }




    #region Event Handlers

    private void HostGame_Click(object sender, RoutedEventArgs e)
    {
        // Host chooses color
        HostScreen hostScreen = new(window);
        window.Content = hostScreen;
    }


    private void JoinGame_Click(object sender, RoutedEventArgs e)
    {
        // Color obtained from server based on host's choice
        JoinScreen joinScreen = new(window);
        window.Content = joinScreen;
    }


    private void SearchForGame_Click(object sender, RoutedEventArgs e)
    {
        StartGame(PieceColor.White); // color should be randomly decided by server
    }


    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        window.Close();
    }

    #endregion
}

