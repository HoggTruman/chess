using GameLogic;
using GameLogic.Enums;
using GameLogic.Interfaces;
//using System.Drawing;
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
using System.Xml.Linq;

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class GameWindow : Window
{
    #region Fields

    private readonly Image[,] pieceImages = new Image[Board.BoardSize, Board.BoardSize];
    private readonly Rectangle[,] highlights = new Rectangle[Board.BoardSize, Board.BoardSize];

    private readonly GameManager _gameManager;
    
    /// <summary>
    /// The move's To square as key, and the move as the value.
    /// </summary>
    private Dictionary<(int row, int col), IMove> highlightedMoves = [];

    private readonly SolidColorBrush highlightBrush = new(Color.FromArgb(150, 125, 255, 125));

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

                Rectangle highlight = new();
                highlights[rowIndex, colIndex] = highlight;
                HighlightGrid.Children.Add(highlight);
            }
        }
    }


    private void DrawPieces()
    {
        for (int r = 0; r < Board.BoardSize; r++)
        {
            for (int c = 0; c < Board.BoardSize; c++)
            {
                IPiece? piece = _gameManager.Board.State[r, c];

                if (_gameManager.PlayerColor == PieceColor.White)
                {
                    pieceImages[r, c].Source = Images.GetImageSource(piece);
                }
                else if (_gameManager.PlayerColor == PieceColor.Black)
                {
                    // rotate board if player is black
                    pieceImages[Board.MaxIndex - r, Board.MaxIndex - c].Source = Images.GetImageSource(piece);
                }
            }
        }
    }

    #endregion

    private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        //if (_gameManager.ActivePlayerColor != _gameManager.PlayerColor)
        //{
        //    return;
        //}

        Point point = e.GetPosition(BoardGrid);
        (int row, int col) square = PointToSquare(point);

        if (_gameManager.PlayerColor == PieceColor.Black)
        {
            // Adjust for Black player board rotation
            square = RotateSquare180(square);
        }

        if (highlightedMoves.Count == 0)
        {
            var moveOptions = _gameManager.ActivePlayerMoves[square.row, square.col];

            if (moveOptions != null && moveOptions.Count > 0)
            {    
                foreach (var move in moveOptions)
                {
                    highlightedMoves[move.To] = move;
                }

                HightlightSquares(highlightedMoves.Keys);
            }
        }
        else
        {
            if (highlightedMoves.TryGetValue(square, out IMove? move))
            {
                // PROMOTION HANDLING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                _gameManager.SwitchActivePlayer();
                _gameManager.HandleMove(move);
                DrawPieces();
                _gameManager.UpdateActivePlayerMoves();
                // SEND MOVE TO SERVER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }

            ClearHighlights();
            highlightedMoves = [];
        }
    }


    private void HightlightSquares(IEnumerable<(int row, int col)> squares)
    {
        foreach (var square in squares)
        {
            var (row, col) = square;

            if (_gameManager.PlayerColor == PieceColor.Black)
            {
                (row, col) = RotateSquare180((row, col));
            }

            highlights[row, col].Fill = highlightBrush;
        }
    }


    private void ClearHighlights()
    {
        for (int r = 0; r < Board.BoardSize; r++)
        {
            for (int c = 0; c < Board.BoardSize; c++)
            {
                highlights[r, c].Fill = null; 
            }
        }
    }

    private (int row, int col) PointToSquare(Point point)
    {
        double squareSize = BoardGrid.ActualWidth / Board.BoardSize;
        int row = (int)(point.Y / squareSize);
        int col = (int)(point.X / squareSize);
        return (row, col);
    }


    private (int row, int col) RotateSquare180((int row, int col) s)
    {
        return (Board.MaxIndex - s.row, Board.MaxIndex - s.col);
    }


}