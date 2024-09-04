using GameLogic;
using GameLogic.Interfaces;
using System.Text;
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
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class GameWindow : Window
{
    #region Fields

    private readonly Image[,] pieceImages = new Image[Board.BoardSize, Board.BoardSize];

    private readonly GameManager _gameManager;

    #endregion



    #region Constructors

    public GameWindow(GameManager gameManager)
    {
        InitializeComponent();
        _gameManager = gameManager;
        InitializePieceImages();
        DrawPieces();
    }

    #endregion



    #region Private Methods

    private void InitializePieceImages()
    {
        for (int rowIndex = 0; rowIndex < Board.BoardSize; rowIndex++)
        {
            for (int colIndex = 0; colIndex < Board.BoardSize; colIndex++)
            {
                Image image = new();
                pieceImages[rowIndex, colIndex] = image;
                PieceGrid.Children.Add(image);
            }
        }
    }


    private void DrawPieces()
    {
        for (int rowIndex = 0; rowIndex < Board.BoardSize; rowIndex++)
        {
            for (int colIndex = 0; colIndex < Board.BoardSize; colIndex++)
            {
                IPiece? piece = _gameManager.Board.State[rowIndex, colIndex];
                pieceImages[rowIndex, colIndex].Source = Images.GetImageSource(piece);
            }
        }
    }

    #endregion
}