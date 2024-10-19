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
    private readonly PieceColor _playerColor;

    /// <summary>
    /// Event fired when GameOverMenu "Exit" button is clicked. 
    /// </summary>
    public event Action? ExitClicked;

    /// <summary>
    /// Event fired when GameOverMenu "Play Again" button is clicked. 
    /// </summary>
    public event Action? PlayAgainClicked;




    public GameOverMenu(GameManager gameManager, PieceColor playerColor)
    {
        InitializeComponent();
        _gameManager = gameManager;
        _playerColor = playerColor;

        var (winner, reason) = gameManager.GetGameResult();
        WinnerText.Text = GetWinnerText(winner);
        ReasonText.Text = GetReasonText(reason);
    }




    public string GetWinnerText(PieceColor winner)
    {
        if (_playerColor == PieceColor.None)
        {
            return winner switch
            {
                PieceColor.White => "White Wins!",
                PieceColor.Black => "Black Wins!",
                _ => "It's A Draw!"
            };
        }
        else if (_playerColor == winner)
        {
            return "You Win!";
        }
        else if (_playerColor == ColorHelpers.Opposite(winner))
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

