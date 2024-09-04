using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Moves;

namespace GameLogic;

public class GameManager
{
    #region Fields

    //private readonly Board _board = new();
    private readonly PieceColor _playerColor; // underscore???!?!?!?

    #endregion



    #region Properties

    public Board Board { get; } = new();

    public PieceColor ActivePlayerColor { get; set; } = PieceColor.White;

    #endregion



    #region Constructors

    public GameManager(PieceColor playerColor)
    {        
        _playerColor = playerColor;
        Board.Initialize();
    }

    #endregion



    #region Public Methods

    public List<IMove>?[,] GetPlayerMoves(PieceColor color)
    {
        List<IMove>?[,] boardMoves = new List<IMove>?[Board.BoardSize, Board.BoardSize];

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

    #endregion

}