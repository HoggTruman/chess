using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;

namespace GameLogic;

public class Board
{
    #region constants

    public const int BoardSize = 8;
    public const int MinIndex = 0;
    public const int MaxIndex = BoardSize - 1;

    #endregion 

    #region fields

    private List<IPiece> _pieces;

    #endregion

    #region properties

    public List<IPiece> Pieces
    {
        get => _pieces;
    }

    public IPiece?[,] State { get; set; }

    #endregion

    #region constructor

    public Board(List<IPiece> pieces = null!)
    {
        _pieces = pieces ?? new List<IPiece>();
        State = new IPiece?[BoardSize, BoardSize];

        foreach (var piece in _pieces)
        {
            State[piece.Row, piece.Col] = piece;
        }
    }

    #endregion

    #region public methods

    public List<(int row, int col)> GetValidMoves(IPiece piece) 
    {
        List<(int row, int col)> validMoves = [];

        // get all targeted squares
        List<(int row, int col)> allMoves = piece.GetReachableSquares(this);

        // keep the squares which represent a valid move
        foreach (var move in allMoves)
        {
            if (MoveLeavesPlayerInCheck((piece.Row, piece.Col), move) == false)
            {
                validMoves.Add(move);
            }
        }

        return validMoves;
    }


    // public void MovePiece((int row, int col) from, (int row, int col) to)
    // {
    //     var movingPiece = State[from.row, from.col];

    //     if (movingPiece == null)
    //         throw new ArgumentException("No piece at from location");

    //     State[to.row, to.col] = movingPiece;
    //     State[from.row, from.col] = null;
        
    //     movingPiece.Row = to.row;
    //     movingPiece.Col = to.col;
    // }


    public bool UnderCheck(Color color)
    {
        var king = GetKingByColor(color);

        if (king == null)
        { 
            throw new ArgumentException($"No {color} king can be found");
        }

        var enemyPieces = GetPiecesByColor(ColorHelpers.OppositeColor(color));

        return enemyPieces.Any(piece => piece.GetTargetedSquares(this).Contains(king.Square));
    }

    #endregion

    #region private methods

    private List<IPiece> GetPiecesByColor(Color color)
    {
        return _pieces.Where(x => x.Color == color).ToList();
    }


    private IPiece? GetKingByColor(Color color)
    {
        return _pieces.FirstOrDefault(x => 
            x.PieceType == PieceType.King &&
            x.Color == color
        );
    }


    private bool MoveLeavesPlayerInCheck((int row, int col) from, (int row, int col) to)
    {
        IPiece? movingPiece = State[from.row, from.col];
        IPiece? targetPiece = State[to.row, to.col];

        if (movingPiece == null)
            throw new ArgumentException("Piece not found at specified from square");

        // make the move
        State[from.row, from.col] = null;
        State[to.row, to.col] = movingPiece;
        movingPiece.Row = to.row;
        movingPiece.Col = to.col;

        if (targetPiece != null)
            _pieces.Remove(targetPiece);

        // record the result
        bool result = UnderCheck(movingPiece.Color);

        // roll back pieces
        State[from.row, from.col] = movingPiece;
        movingPiece.Row = from.row;
        movingPiece.Col = from.col;
        State[to.row, to.col] = targetPiece;

        if (targetPiece != null)
            _pieces.Add(targetPiece);

        
        return result;
    }

    #endregion
}