using BetterGameLogic.Enums;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameApplication.Windows.Game;

/// <summary>
/// Interaction logic for PromotionMenu.xaml
/// </summary>
public partial class PromotionMenu : UserControl
{
    public event Action<PieceType>? PieceClicked;


    public PromotionMenu(PieceColor color)
    {
        InitializeComponent();
        QueenImage.Source = Images.GetImageSource(PieceType.Queen, color);
        RookImage.Source = Images.GetImageSource(PieceType.Rook, color);
        KnightImage.Source = Images.GetImageSource(PieceType.Knight, color);
        BishopImage.Source = Images.GetImageSource(PieceType.Bishop, color);
    }


    private void QueenImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        PieceClicked?.Invoke(PieceType.Queen);
    }


    private void RookImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        PieceClicked?.Invoke(PieceType.Rook);
    }


    private void KnightImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        PieceClicked?.Invoke(PieceType.Knight);
    }


    private void BishopImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        PieceClicked?.Invoke(PieceType.Bishop);
    }
}

