﻿using GameLogic;
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

    /// <summary>
    /// The GameManager for the current game.
    /// </summary>
    private readonly GameManager gameManager;

    /// <summary>
    /// A 2D array with piece Images in positions corresponding to the Board.
    /// </summary>
    private readonly Image[,] pieceImages = new Image[Board.BoardSize, Board.BoardSize];

    /// <summary>
    /// A 2D array of Rectangles.
    /// The Fill of a Rectangle is modified to represent an available move.
    /// </summary>
    private readonly Rectangle[,] highlights = new Rectangle[Board.BoardSize, Board.BoardSize];

    /// <summary>
    /// A SolidColorBrush used to set the Fill for highlighted squares.
    /// </summary>
    private readonly SolidColorBrush highlightBrush = new(Color.FromArgb(150, 125, 255, 125));

    /// <summary>
    /// A Dictionary with To square as key, and the corresponding IMove as the value.
    /// </summary>
    private Dictionary<(int row, int col), IMove> highlightedMoves = [];

    /// <summary>
    /// A bool to dictate whether the player's clicks should interact with the board at all.
    /// </summary>
    private bool FrozenBoard = false;

    #endregion



    #region Constructors

    public GameWindow(GameManager gameManager)
    {
        InitializeComponent();
        this.gameManager = gameManager;
        InitializeGrids();
        DrawPieces();
    }

    #endregion



    #region Private Methods

    /// <summary>
    /// Initializes pieceImages and highlights arrays.
    /// Adds the contents as children to the corresponding GameWindow grids.
    /// </summary>
    private void InitializeGrids()
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


    /// <summary>
    /// Updates pieceImages to match the Board from the perspective of the player
    /// (The player's pieces are at the bottom of the board)
    /// </summary>
    private void DrawPieces()
    {
        for (int r = 0; r < Board.BoardSize; r++)
        {
            for (int c = 0; c < Board.BoardSize; c++)
            {
                IPiece? piece = gameManager.Board.State[r, c];

                if (gameManager.PlayerColor == PieceColor.White)
                {
                    pieceImages[r, c].Source = Images.GetImageSource(piece);
                }
                else if (gameManager.PlayerColor == PieceColor.Black)
                {
                    // rotate board if player is black
                    var (row, col) = RotateSquare180((r, c));
                    pieceImages[row, col].Source = Images.GetImageSource(piece);
                }
            }
        }
    }


    /// <summary>
    /// Handles MouseDown on the BoardGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (FrozenBoard)
        {
            return;
        }
        //if (_gameManager.ActivePlayerColor != _gameManager.PlayerColor)
        //{
        //    return;
        //}

        Point point = e.GetPosition(BoardGrid);
        (int row, int col) square = PointToSquare(point);

        if (gameManager.PlayerColor == PieceColor.Black)
        {
            // Adjust for Black player board rotation
            square = RotateSquare180(square);
        }

        if (highlightedMoves.Count == 0)
        {
            var moveOptions = gameManager.ActivePlayerMoves[square.row, square.col];

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
                gameManager.HandleMove(move);
                DrawPieces();
            
                gameManager.SwitchTurn();

                if (gameManager.GameIsOver())
                {
                    HandleGameOver();
                }
                // SEND MOVE TO SERVER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }

            ClearHighlights();
            highlightedMoves = [];
        }
    }


    /// <summary>
    /// Updates the GameWindow when the game is over.
    /// </summary>
    private void HandleGameOver()
    {
        FrozenBoard = true;
        GameOverMenu gameOverMenu = new(gameManager);
        MenuContainer.Content = gameOverMenu;
    }


    /// <summary>
    /// Highlights the provided squares.
    /// </summary>
    /// <param name="squares"></param>
    private void HightlightSquares(IEnumerable<(int row, int col)> squares)
    {
        foreach (var square in squares)
        {
            var (row, col) = square;

            if (gameManager.PlayerColor == PieceColor.Black)
            {
                (row, col) = RotateSquare180((row, col));
            }

            highlights[row, col].Fill = highlightBrush;
        }
    }


    /// <summary>
    /// Clears all highlights on the Board.
    /// </summary>
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


    /// <summary>
    /// Converts a Point clicked to a corresponding square on the Board.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private (int row, int col) PointToSquare(Point point)
    {
        double squareSize = BoardGrid.ActualWidth / Board.BoardSize;
        int row = (int)(point.Y / squareSize);
        int col = (int)(point.X / squareSize);
        return (row, col);
    }


    /// <summary>
    /// Returns the square rotated 180 degrees.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>(row, col) of the rotated square.</returns>
    private (int row, int col) RotateSquare180((int row, int col) s)
    {
        return (Board.MaxIndex - s.row, Board.MaxIndex - s.col);
    }

    #endregion
}