using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogic;

public class GameManager
{
    #region Properties

    /// <summary>
    /// The Board of the active game.
    /// </summary>
    public Board Board { get; private set; } = new();

    /// <summary>
    /// The PieceColor of the player whose turn it is.
    /// </summary>
    public PieceColor ActivePlayerColor { get; set; } = PieceColor.White;

    /// <summary>
    /// A cached 2D array of the available moves to the player whose turn it is.
    /// </summary>
    public List<IMove>?[,] ActivePlayerMoves { get; private set; }

    /// <summary>
    /// True if the active player is under check.
    /// </summary>
    public bool ActivePlayerUnderCheck { get; private set; }

    /// <summary>
    /// The KingPiece of the active player.
    /// </summary>
    public KingPiece ActivePlayerKing
    {
        get => Board.GetKing(ActivePlayerColor);
    }

    #endregion



    #region Constructors

    public GameManager(Board? board = null)
    {        
        // Avoids using StartNewGame so that a manually set up Board can be used in testing
        if (board == null)
        {
            Board = new Board();
            Board.Initialize();
        }
        else
        {
            Board = board;
        }
        
        ActivePlayerMoves = GetPlayerMoves(ActivePlayerColor);
        ActivePlayerUnderCheck = Board.GetKing(ActivePlayerColor).IsChecked();
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Sets up a new game.
    /// </summary>
    public void StartNewGame()
    {
        Board = new();
        Board.Initialize();
        ActivePlayerColor = PieceColor.White;
        ActivePlayerMoves = GetPlayerMoves(ActivePlayerColor);
        ActivePlayerUnderCheck = Board.GetKing(ActivePlayerColor).IsChecked();
    }


    /// <summary>
    /// Returns a 2D array containing a List of available moves if the corresponding
    /// board square contains one of the player's pieces, or null if it doesn't. 
    /// at the corresponding square on the board. 
    /// </summary>
    /// <param name="color">The color of the player</param>
    /// <returns>A 2D Array of nullable Lists of Moves</returns>
    public List<IMove>?[,] GetPlayerMoves(PieceColor color)
    {
        if (color == PieceColor.None)
        {
            throw new ArgumentException($"Can not get moves for player with color {color}");
        }

        var boardMoves = new List<IMove>?[Board.BoardSize, Board.BoardSize];

        foreach (var piece in Board.Pieces[color])
        {
            boardMoves[piece.Row, piece.Col] = piece.GetValidMoves();
        }

        return boardMoves;
    }


    /// <summary>
    /// Checks if a move is valid.
    /// </summary>
    /// <param name="move">An IMove to test.</param>
    /// <returns>true for a valid move. Otherwise, false</returns>
    public bool IsValidMove(IMove move)
    {
        var validMoves = ActivePlayerMoves[move.From.row, move.From.col];

        if (validMoves == null)
        {
            return false;
        }

        return validMoves.Any(x => x.IsEquivalentTo(move));
    }


    /// <summary>
    /// Updates the board based on the provided move.
    /// </summary>
    /// <param name="move">A move to carry out</param>
    public void HandleMove(IMove move)
    {
        if (move.MoveType == MoveType.Standard)
        {
            Board.StandardMove((StandardMove)move);
        }
        else if (move.MoveType == MoveType.EnPassant)
        {
            Board.EnPassant((EnPassantMove)move);
        }
        else if (move.MoveType == MoveType.Promotion)
        {
            Board.PawnPromote((PromotionMove)move);
        }
        else if (move.MoveType == MoveType.Castle)
        {
            Board.Castle((CastleMove)move);
        }
    }


    /// <summary>
    /// Swtiches the PieceColor of the ActivePlayerColor.
    /// Updates ActivePlayerMoves ready for the next player.
    /// </summary>
    public void SwitchTurn()
    {
        ActivePlayerColor = ColorHelpers.Opposite(ActivePlayerColor);
        ActivePlayerMoves = GetPlayerMoves(ActivePlayerColor);
        ActivePlayerUnderCheck = Board.GetKing(ActivePlayerColor).IsChecked();
    }


    /// <summary>
    /// Determines whether the game is finished.
    /// </summary>
    /// <returns>true if the game is over. Otherwise, false</returns>
    public bool GameIsOver()
    {
        // Only the two kings are left returns true
        if (Board.Pieces[PieceColor.White].Count == 1 &&
            Board.Pieces[PieceColor.Black].Count == 1)
        {
            return true;
        }

        // If the active player has a valid move, returns false
        for (int r = 0; r < Board.BoardSize; r++)
        {
            for (int c = 0; c < Board.BoardSize; c++)
            {
                var squareMoves = ActivePlayerMoves[r, c];

                if (squareMoves != null &&
                    squareMoves.Count > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Determines the winner of the game (or if it is a draw) and the reason for the game ending.
    /// </summary>
    /// <returns>A tuple of the PieceColor of the winner and a GameOverReason</returns>
    /// <exception cref="Exception">The game is not over</exception>
    public (PieceColor, GameOverReason) GetGameResult()
    {
        if (GameIsOver() == false)
        {
            throw new Exception("The Game is not over so a winner can not be determined.");
        }

        if (ActivePlayerUnderCheck)
        {
            // Checkmate
            var winner = ColorHelpers.Opposite(ActivePlayerColor);
            return (winner, GameOverReason.Checkmate);
        }
        else
        {
            // Insufficient Material
            if (Board.Pieces[PieceColor.White].Count == 1 &&
                Board.Pieces[PieceColor.Black].Count == 1)
            {
                return (PieceColor.None, GameOverReason.InsufficientMaterial);
            }

            // Stalemate
            return (PieceColor.None, GameOverReason.Stalemate);
        }
    }    

    #endregion

}