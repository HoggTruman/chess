using GameLogic.Enums;
using System.Windows;
using System.Windows.Controls;

namespace GameApplication.Windows.Game;


/// <summary>
/// Interaction logic for GameOverMenu.xaml
/// </summary>
public partial class GameOverMenu : UserControl
{
    #region Events

    /// <summary>
    /// Event fired when GameOverMenu "Exit" button is clicked. 
    /// </summary>
    public event Action? ExitClicked;

    #endregion



    #region Text

    public const string WhiteWinsText = "White Wins!";
    public const string BlackWinsText = "Black Wins!";
    public const string YouWinText = "You Win!";
    public const string YouLoseText = "You Lose!";
    public const string NoWinnerText = "It's A Draw!";

    #endregion




    public GameOverMenu(PieceColor winnerColor, GameOverReason reason, PieceColor playerColor)
    {
        InitializeComponent();

        WinnerText.Text = GetWinnerText(winnerColor, playerColor);
        ReasonText.Text = GetReasonText(reason);
    }




    public static string GetWinnerText(PieceColor winnerColor, PieceColor playerColor)
    {
        return playerColor switch
        {
            PieceColor.None => winnerColor switch
            {
                PieceColor.White => WhiteWinsText,
                PieceColor.Black => BlackWinsText,
                _ => NoWinnerText
            },
            PieceColor.White => winnerColor switch
            {
                PieceColor.White => YouWinText,
                PieceColor.Black => YouLoseText,
                _ => NoWinnerText
            },
            PieceColor.Black => winnerColor switch
            {
                PieceColor.White => YouLoseText,
                PieceColor.Black => YouWinText,
                _ => NoWinnerText
            },
            _ => throw new ArgumentException($"Can not determine winner with winnerColor: {winnerColor}, playerColor: {playerColor}."),
        };
    }


    public static string GetReasonText(GameOverReason reason)
    {
        return reason switch
        {
            GameOverReason.Checkmate => "Checkmate",
            GameOverReason.Stalemate => "Stalemate",
            GameOverReason.InsufficientMaterial => "Insufficient Material",
            GameOverReason.Disconnect => "A Communication Error Occurred",
            _ => ""
        };
    }


    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        ExitClicked?.Invoke();
    }
}

