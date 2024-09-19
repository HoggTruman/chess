using GameLogic;
using GameLogic.Enums;
using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Game;


/// <summary>
/// Interaction logic for GameOverMenu.xaml
/// </summary>
public partial class GameOverMenu : UserControl
{
    private readonly GameManager gameManager;

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
        this.gameManager = gameManager;

        var (winner, reason) = gameManager.GetGameResult();
        WinnerText.Text = GetWinnerText(winner);
        ReasonText.Text = GetReasonText(reason);
    }




    public string GetWinnerText(PieceColor? winner)
    {
        if (winner == null)
        {
            return "It's a Draw!";
        }
        else if (winner == gameManager.PlayerColor)
        {
            return "You Win!";
        }
        else
        {
            return "You Lose!";
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

