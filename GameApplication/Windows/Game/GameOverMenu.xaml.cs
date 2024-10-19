using GameLogic.Enums;
using GameLogic.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Game;


/// <summary>
/// Interaction logic for GameOverMenu.xaml
/// </summary>
public partial class GameOverMenu : UserControl
{
    /// <summary>
    /// Event fired when GameOverMenu "Exit" button is clicked. 
    /// </summary>
    public event Action? ExitClicked;

    /// <summary>
    /// Event fired when GameOverMenu "Play Again" button is clicked. 
    /// </summary>
    public event Action? PlayAgainClicked;




    public GameOverMenu(PieceColor winnerColor, GameOverReason reason, PieceColor playerColor)
    {
        InitializeComponent();

        WinnerText.Text = GetWinnerText(winnerColor, playerColor);
        ReasonText.Text = GetReasonText(reason);
    }




    public static string GetWinnerText(PieceColor winnerColor, PieceColor playerColor)
    {
        if (playerColor == PieceColor.None)
        {
            return winnerColor switch
            {
                PieceColor.White => "White Wins!",
                PieceColor.Black => "Black Wins!",
                _ => "It's A Draw!"
            };
        }
        else if (playerColor == winnerColor)
        {
            return "You Win!";
        }
        else if (playerColor == ColorHelpers.Opposite(winnerColor))
        {
            return "You Lose!";
        }
        else 
        {
            return "It's A Draw!";
        }        
    }


    public static string GetReasonText(GameOverReason reason)
    {
        return reason switch
        {
            GameOverReason.Checkmate => "Checkmate",
            GameOverReason.Stalemate => "Stalemate",
            GameOverReason.InsufficientMaterial => "Insufficient Material",
            _ => ""
        };
    }


    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        ExitClicked?.Invoke();
    }

    private void PlayAgain_Click(object sender, RoutedEventArgs e)
    {
        PlayAgainClicked?.Invoke();
    }
}

