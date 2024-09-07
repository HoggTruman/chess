using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;

namespace GameLogic;

public class GameManager
{
    #region Properties

    public Board Board { get; } = new();

    public PieceColor PlayerColor { get; }

    public PieceColor ActivePlayerColor { get; set; } //= PieceColor.White;

    public List<IMove>?[,] ActivePlayerMoves { get; private set; }

    #endregion



    #region Constructors

    public GameManager(PieceColor playerColor)
    {        
        Board.Initialize();
        PlayerColor = playerColor;
        ActivePlayerColor = playerColor; // ActivePlayerColor set to the player's color to make testing things easier for now
        ActivePlayerMoves = GetPlayerMoves(playerColor);
    }

    #endregion



    #region Public Methods

    public List<IMove>?[,] GetPlayerMoves(PieceColor color)
    {
        var boardMoves = new List<IMove>?[Board.BoardSize, Board.BoardSize];

        foreach (var piece in Board.Pieces[color])
        {
            boardMoves[piece.Row, piece.Col] = piece.GetValidMoves();
        }

        return boardMoves;
    }


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


    public void SwitchActivePlayer()
    {
        ActivePlayerColor = ColorHelpers.Opposite(ActivePlayerColor);
    }


    public void UpdateActivePlayerMoves()
    {
        ActivePlayerMoves = GetPlayerMoves(ActivePlayerColor);
    }


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