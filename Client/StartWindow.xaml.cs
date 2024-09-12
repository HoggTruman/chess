using GameLogic;
using GameLogic.Enums;
using System.Windows;


namespace Client;


/// <summary>
/// Interaction logic for StartWindow.xaml
/// </summary>
public partial class StartWindow : Window
{
    public StartWindow()
    {
        InitializeComponent();
        //Visibility = Visibility.Hidden;

    }


    private void StartGame(PieceColor playerColor)
    {
        Board board = new();
        board.Initialize();
        GameManager gameManager = new(board, playerColor); 

        GameWindow gameWindow = new(gameManager);
        gameWindow.Show();
        Close();
    }


    private void HostGame_Click(object sender, RoutedEventArgs e)
    {
        // Host chooses color
    }


    private void JoinGame_Click(object sender, RoutedEventArgs e)
    {
        // Color obtained from server based on host's choice
    }


    private void SearchForGame_Click(object sender, RoutedEventArgs e)
    {
        StartGame(PieceColor.White); // color should be randomly decided by server
    }


    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}