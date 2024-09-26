using GameLogic;
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
    private readonly GameManager _gameManager;

    /// <summary>
    /// Event fired when GameOverMenu "Exit" button is clicked. 
    /// </summary>
    public event Action? ExitClicked;

    /// <summary>
    /// Event fired when GameOverMenu "Play Again" button is clicked. 
    /// </summary>
    public event Action? PlayAgainClicked;




    public GameOverMenu(GameManager gameManager)
    {
        InitializeComponent();
        _gameManager = gameManager;

        var (winner, reason) = gameManager.GetGameResult();
        WinnerText.Text = GetWinnerText(winner);
        ReasonText.Text = GetReasonText(reason);
    }




    public string GetWinnerText(PieceColor winner)
    {
        if (_gameManager.PlayerColor == PieceColor.None)
        {
            return winner switch
            {
                PieceColor.White => "White Wins!",
                PieceColor.Black => "Black Wins!",
                _ => "It's A Draw!"
            };
        }
        else if (_gameManager.PlayerColor == winner)
        {
            return "You Win!";
        }
        else if (_gameManager.PlayerColor == ColorHelpers.Opposite(winner))
        {
            return "You Lose!";
        }
        else 
        {
            return "It's A Draw!";
        }        
    }


    public string GetReasonText(GameOverReason reason)
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

