using Client;
using GameApplication.Windows.Start;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameApplication.Windows.Game;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class GameWindow : Window
{
    #region Fields

    /// <summary>
    /// The GameManager for the current game.
    /// </summary>
    private readonly GameManager _gameManager;

    /// <summary>
    /// The PieceColor of the player.
    /// </summary>
    private readonly PieceColor _playerColor;

    /// <summary>
    /// The GameClient for an online game or null for a local game.
    /// </summary>
    private readonly GameClient? _gameClient;


    /// <summary>
    /// A 2D array with piece Images in positions corresponding to the Board.
    /// </summary>
    private readonly Image[,] _pieceImages = new Image[Board.BoardSize, Board.BoardSize];

    /// <summary>
    /// A 2D array of Rectangles.
    /// The Fill of a Rectangle is modified to represent an available move.
    /// </summary>
    private readonly Rectangle[,] _highlights = new Rectangle[Board.BoardSize, Board.BoardSize];


    /// <summary>
    /// The square of the currently selected Piece of the player.
    /// </summary>
    private (int row, int col)? _selectedSquare; 

    /// <summary>
    /// A Dictionary with To square as key, and the corresponding IMove as the value.
    /// </summary>
    private Dictionary<(int row, int col), IMove> _selectedPieceMoveOptions = [];

    /// <summary>
    /// A bool to dictate whether the player's clicks should interact with the board at all.
    /// </summary>
    private bool _frozenBoard = false;


    /// <summary>
    /// A SolidColorBrush used to set the Fill for highlighted squares.
    /// </summary>
    private static readonly SolidColorBrush HighlightBrush = new(Color.FromArgb(150, 125, 255, 125));

    /// <summary>
    /// A SolidColorBrush used to set the Fill for the king's square when under check.
    /// </summary>
    private static readonly SolidColorBrush SelectBrush = new(Color.FromArgb(200, 255, 255, 125));

    /// <summary>
    /// A SolidColorBrush used to set the Fill for the king's square when under check.
    /// </summary>
    private static readonly SolidColorBrush CheckBrush = new(Color.FromArgb(175, 255, 20, 20));

    #endregion



    #region Constructors

    public GameWindow(GameManager gameManager, PieceColor playerColor, GameClient? gameClient = null)
    {
        InitializeComponent();        
        _gameManager = gameManager;
        _playerColor = playerColor;
        _gameClient = gameClient;

        if (_gameClient != null)
        {
            _frozenBoard = playerColor == PieceColor.Black;
            _gameClient.MoveReceived += HandleMove;
            _gameClient.RoomClosed += pieceColor => HandleGameOver(pieceColor, GameOverReason.Disconnect);
        }        

        InitializeGrids();
        DrawPieces();
    }

    #endregion



    #region Event Handlers

    /// <summary>
    /// Handles MouseDown on the BoardGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_frozenBoard)
        {
            return;
        }

        Point point = e.GetPosition(BoardGrid);
        (int row, int col) square = PointToSquare(point);        

        if (_selectedSquare == null)
        {
            var moveOptions = _gameManager.ActivePlayerMoves[square.row, square.col];
            
            if (moveOptions != null)
            {   
                _selectedSquare = square;
                HighlightSquare(square, SelectBrush);

                foreach (var move in moveOptions)
                {
                    _selectedPieceMoveOptions[move.To] = move;
                    HighlightSquare(move.To, HighlightBrush);
                }
            }
        }
        else if (_selectedPieceMoveOptions.TryGetValue(square, out IMove? move))
        {
            if (move.MoveType == MoveType.Promotion)
            {
                HandlePromotionMove(move);
            }
            else
            {
                if (_gameClient != null)
                {
                    _frozenBoard = true;
                    await _gameClient.SendMove(move);                    
                }   
                HandleMove(move);             
            }
        }
        else
        {
            // A non-highlighted square is clicked while a piece is selected
            _selectedSquare = null;
            _selectedPieceMoveOptions = [];
            ClearHighlights();
            if (_gameManager.ActivePlayerUnderCheck)
            {
                var king = _gameManager.Board.Kings[_gameManager.ActivePlayerColor];
                HighlightSquare(king.Square, CheckBrush);
            }
        }      
    }

    #endregion



    #region Game Methods

    /// <summary>
    /// Updates the game for the provided move.
    /// </summary>
    /// <param name="move">The IMove to apply.</param>
    private void HandleMove(IMove move)
    {
        // Update pieces
        _gameManager.HandleMove(move);
        _gameManager.SwitchTurn();
        DrawPieces();

        // Remove current highlights and selection
        _selectedSquare = null;
        _selectedPieceMoveOptions = [];
        ClearHighlights();        

        // Highlight the next player's king square if under check
        if (_gameManager.ActivePlayerUnderCheck)
        {
            var king = _gameManager.Board.Kings[_gameManager.ActivePlayerColor];
            HighlightSquare(king.Square, CheckBrush);
        }

        if (_gameClient != null)
        {
            _frozenBoard = _playerColor != _gameManager.ActivePlayerColor;
        }

        if (_gameManager.GameIsOver())
        {
            var (winnerColor, reason) = _gameManager.GetGameResult();
            HandleGameOver(winnerColor, reason);
        }
    }    


    /// <summary>
    /// Creates a menu on screen for the player to select the piece to promote to.
    /// Updates the board based on the player's chocie.
    /// </summary>
    /// <param name="move"></param>
    private void HandlePromotionMove(IMove move)
    {
        _frozenBoard = true;
        PromotionMenu promotionMenu = new(_gameManager.ActivePlayerColor);
        MenuContainer.Content = promotionMenu;
        promotionMenu.PieceClicked += async pieceType =>
        {
            _frozenBoard = false;
            move = new PromotionMove(move.From, move.To, pieceType);
            MenuContainer.Content = null;
            if (_gameClient != null)
            {
                _frozenBoard = true;
                await _gameClient.SendMove(move);
            }
            HandleMove(move);
        };
    }


    /// <summary>
    /// Updates the GameWindow when the game is over.
    /// </summary>
    private async void HandleGameOver(PieceColor winnerColor, GameOverReason reason)
    {
        if (MenuContainer.Content is GameOverMenu)
        {
            return;
        }

        _frozenBoard = true;
        MenuContainer.Content = null; // Remove promotion menu        
        ClearHighlights();

        if (_gameClient != null)
        {
            await _gameClient.StopListening();
            _gameClient.Dispose();
        }

        GameOverMenu gameOverMenu = new(winnerColor, reason, _playerColor);
        MenuContainer.Content = gameOverMenu;

        gameOverMenu.ExitClicked += () =>
        {        
            StartWindow startWindow = new();
            startWindow.Show();     
            Close();                   
        };
    }

    #endregion



    #region Drawing Methods

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
                _pieceImages[rowIndex, colIndex] = image;
                PieceGrid.Children.Add(image);

                Rectangle highlight = new();
                _highlights[rowIndex, colIndex] = highlight;
                HighlightGrid.Children.Add(highlight);
            }
        }
    }


    /// <summary>
    /// Updates pieceImages to match the Board from the perspective of the player.
    /// Takes White's perspective for local games.
    /// (The player's pieces are at the bottom of the board)
    /// </summary>
    private void DrawPieces()
    {
        for (int r = 0; r < Board.BoardSize; r++)
        {
            for (int c = 0; c < Board.BoardSize; c++)
            {
                IPiece? piece = _gameManager.Board.State[r, c];

                if (_playerColor == PieceColor.Black)
                {
                    // rotate board if player is black
                    var (row, col) = BoardHelpers.RotateSquare180((r, c));
                    _pieceImages[row, col].Source = Images.GetImageSource(piece);
                }
                else 
                {
                    _pieceImages[r, c].Source = Images.GetImageSource(piece);
                }
            }
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
                _highlights[r, c].Fill = null; 
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

        if (_playerColor == PieceColor.Black)
        {
            (row, col) = BoardHelpers.RotateSquare180(square);
        }

        _highlights[row, col].Fill = brush;
    }    

    #endregion



    #region Helpers

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

        // Adjust for Black player board rotation
        if (_playerColor == PieceColor.Black)
        {
            (row, col) = BoardHelpers.RotateSquare180((row, col));
        }

        return (row, col);
    }

    #endregion
}