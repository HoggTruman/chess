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
        ActivePlayerMoves = GetPlayerMoves(ActivePlayerColor);
    }

    #endregion

}