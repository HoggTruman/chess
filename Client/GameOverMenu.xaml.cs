using GameLogic;
using GameLogic.Enums;
using GameLogic.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client;

    
/// <summary>
/// Interaction logic for GameOverMenu.xaml
/// </summary>
public partial class GameOverMenu : UserControl
{
    private readonly GameManager gameManager;

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

    }

    private void PlayAgain_Click(object sender, RoutedEventArgs e)
    {

    }
}

