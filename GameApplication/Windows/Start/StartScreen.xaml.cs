using GameApplication.Windows.Game;
using BetterGameLogic;
using BetterGameLogic.Enums;
using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Start;

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
        GameManager gameManager = new(); 
        GameWindow gameWindow = new(gameManager, playerColor);
        gameWindow.Show();
        window.Close();
    }



    #region Event Handlers

    private void HostGame_Click(object sender, RoutedEventArgs e)
    {
        HostScreen hostScreen = new(window);
        window.Content = hostScreen;
    }


    private void JoinGame_Click(object sender, RoutedEventArgs e)
    {
        JoinScreen joinScreen = new(window);
        window.Content = joinScreen;
    }


    private void StartLocal_Click(object sender, RoutedEventArgs e)
    {
        StartGame(PieceColor.White);
    }


    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        window.Close();
    }

    #endregion
}

