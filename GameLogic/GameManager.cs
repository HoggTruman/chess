using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;

namespace GameLogic;

public class GameManager
{
    #region Properties

    /// <summary>
    /// The Board of the active game.
    /// </summary>
    public Board Board { get; } = new();

    /// <summary>
    /// The PieceColor of the player.
    /// </summary>
    public PieceColor PlayerColor { get; }

    /// <summary>
    /// The PieceColor of the player whose turn it is.
    /// </summary>
    public PieceColor ActivePlayerColor { get; set; } //= PieceColor.White;

    /// <summary>
    /// A cached 2D array of the available moves to the player whose turn it is.
    /// </summary>
    public List<IMove>?[,] ActivePlayerMoves { get; private set; }

    #endregion



    #region Constructors

    public GameManager(Board board, PieceColor playerColor)
    {        
        Board = board;
        PlayerColor = playerColor;
        ActivePlayerColor = playerColor; // ActivePlayerColor set to the player's color to make testing things easier for now
        ActivePlayerMoves = GetPlayerMoves(playerColor);
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Returns a 2D array containing a List of available moves if the corresponding
    /// board square contains one of the player's pieces, or null if it doesn't. 
    /// at the corresponding square on the board. 
    /// </summary>
    /// <param name="color">The color of the player</param>
    /// <returns>A 2D Array of nullable Lists of Moves</returns>
    public List<IMove>?[,] GetPlayerMoves(PieceColor color)
    {
        var boardMoves = new List<IMove>?[Board.BoardSize, Board.BoardSize];

        foreach (var piece in Board.Pieces[color])
        {
            boardMoves[piece.Row, piece.Col] = piece.GetValidMoves();
        }

        return boardMoves;
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

        Board.MoveHistory.Add(move);
    }


    /// <summary>
    /// Swtiches the PieceColor of the ActivePlayerColor.
    /// Updates ActivePlayerMoves ready for the next player.
    /// </summary>
    public void SwitchTurn()
    {
        ActivePlayerColor = ColorHelpers.Opposite(ActivePlayerColor);
        ActivePlayerMoves = GetPlayerMoves(ActivePlayerColor);
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
    /// <returns>A tuple of the PieceColor of the winner (or null for a draw) and a GameOverReason</returns>
    /// <exception cref="Exception">The game is not over</exception>
    public (PieceColor?, GameOverReason) GetGameResult()
    {
        if (GameIsOver() == false)
        {
            throw new Exception("The Game is not over so a winner can not be determined.");
        }

        var whiteKing = Board.Kings[PieceColor.White] ?? throw new Exception("Board does not contain a white king");
        var blackKing = Board.Kings[PieceColor.Black] ?? throw new Exception("Board does not contain a black king");

        if (whiteKing.IsChecked())
        {
            // Black wins
            return (PieceColor.Black, GameOverReason.Checkmate);
        }
        else if (blackKing.IsChecked())
        {   
            // White wins
            return (PieceColor.White, GameOverReason.Checkmate);
        }
        else
        {
            // Insufficient Material
            if (Board.Pieces[PieceColor.White].Count == 1 &&
                Board.Pieces[PieceColor.Black].Count == 1)
            {
                return (null, GameOverReason.InsufficientMaterial);
            }

            // Stalemate
            return (null, GameOverReason.Stalemate);
        }
    }    

    #endregion

}