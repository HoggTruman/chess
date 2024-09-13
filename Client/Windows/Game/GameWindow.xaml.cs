using Client.Windows.Start;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;

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

namespace Client.Windows.Game;

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
    /// The square of the currently selected Piece of the player.
    /// </summary>
    private (int row, int col)? selectedSquare; 

    /// <summary>
    /// A Dictionary with To square as key, and the corresponding IMove as the value.
    /// </summary>
    private Dictionary<(int row, int col), IMove> highlightedMoves = [];

    /// <summary>
    /// A bool to dictate whether the player's clicks should interact with the board at all.
    /// </summary>
    private bool frozenBoard = false;


    /// <summary>
    /// A SolidColorBrush used to set the Fill for highlighted squares.
    /// </summary>
    private readonly SolidColorBrush highlightBrush = new(Color.FromArgb(150, 125, 255, 125));

    /// <summary>
    /// A SolidColorBrush used to set the Fill for the king's square when under check.
    /// </summary>
    private readonly SolidColorBrush selectBrush = new(Color.FromArgb(200, 255, 255, 125));

    /// <summary>
    /// A SolidColorBrush used to set the Fill for the king's square when under check.
    /// </summary>
    private readonly SolidColorBrush checkBrush = new(Color.FromArgb(175, 255, 20, 20));

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
                    var (row, col) = BoardHelpers.RotateSquare180((r, c));
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
        if (frozenBoard)
        {
            return;
        }

        Point point = e.GetPosition(BoardGrid);
        (int row, int col) square = PointToSquare(point);

        if (gameManager.PlayerColor == PieceColor.Black)
        {
            // Adjust for Black player board rotation
            square = BoardHelpers.RotateSquare180(square);
        }

        if (selectedSquare == null)
        {
            var moveOptions = gameManager.ActivePlayerMoves[square.row, square.col];
            
            if (moveOptions != null)
            {   
                selectedSquare = square;
                HighlightSquare(square, selectBrush);

                foreach (var move in moveOptions)
                {
                    highlightedMoves[move.To] = move;
                    HighlightSquare(move.To, highlightBrush);
                }
            }
        }
        else if (highlightedMoves.TryGetValue(square, out IMove? move))
        {
            if (move.MoveType == MoveType.Promotion)
            {
                frozenBoard = true;
                PromotionMenu promotionMenu = new(gameManager.ActivePlayerColor);
                MenuContainer.Content = promotionMenu;
                promotionMenu.PieceClicked += pieceType =>
                {
                    frozenBoard = false;
                    move = new PromotionMove(move.From, move.To, pieceType);
                    MenuContainer.Content = null;
                    HandleMove(move);
                    // SEND MOVE TO SERVER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                };
            }
            else
            {
                HandleMove(move);
                // SEND MOVE TO SERVER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
        }
        else
        {
            // A square is clicked that is not a highlighted move while a piece is selected
            selectedSquare = null;
            highlightedMoves = [];
            ClearHighlights();
            if (gameManager.PlayerUnderCheck)
            {
                var king = gameManager.Board.Kings[gameManager.ActivePlayerColor];
                HighlightSquare(king.Square, checkBrush);
            }
        }      
    }


    /// <summary>
    /// Updates the game for the provided move.
    /// </summary>
    /// <param name="move">The IMove to apply.</param>
    private void HandleMove(IMove move)
    {
        // Update pieces
        gameManager.HandleMove(move);
        DrawPieces();

        // Remove current highlights and selection
        selectedSquare = null;
        highlightedMoves = [];
        ClearHighlights();
        
        gameManager.SwitchTurn();

        // Highlight the next player's king square if under check
        if (gameManager.PlayerUnderCheck)
        {
            var king = gameManager.Board.Kings[gameManager.ActivePlayerColor];
            HighlightSquare(king.Square, checkBrush);
        }

        //FrozenBoard = gameManager.ActivePlayerColor != gameManager.PlayerColor;

        if (gameManager.GameIsOver())
        {
            HandleGameOver();
        }
    }


    /// <summary>
    /// Updates the GameWindow when the game is over.
    /// </summary>
    private void HandleGameOver()
    {
        frozenBoard = true;
        ClearHighlights();
        GameOverMenu gameOverMenu = new(gameManager);
        MenuContainer.Content = gameOverMenu;

        gameOverMenu.ExitClicked += () =>
        {            
            StartWindow startWindow = new();
            startWindow.Show();
            Close();
        };

        gameOverMenu.PlayAgainClicked += () =>
        {
            var nextGameColor = ColorHelpers.Opposite(gameManager.PlayerColor);
            gameManager.StartNewGame(nextGameColor);
            DrawPieces();
            frozenBoard = false;
            MenuContainer.Content = null;
        };
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
    /// Highlights the Grid square corresponding to the underlying Board square with the provided brush.
    /// </summary>
    /// <param name="square"></param>
    /// <param name="brush"></param>
    private void HighlightSquare((int row, int col) square, SolidColorBrush brush)
    {
        var (row, col) = square;

        if (gameManager.PlayerColor == PieceColor.Black)
        {
            (row, col) = BoardHelpers.RotateSquare180(square);
        }

        highlights[row, col].Fill = brush;
    }


    /// <summary>
    /// Converts a Point clicked to a corresponding square on the Board.
    /// (Does not account for Black player board rotation)
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

    #endregion
}