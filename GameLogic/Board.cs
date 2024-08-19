using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Pieces;



namespace GameLogic;

public class Board
{
    #region constants

    public const int BoardSize = 8;
    public const int MinIndex = 0;
    public const int MaxIndex = BoardSize - 1;

    #endregion 



    #region properties

    public IPiece?[,] State { get; set; }

    public Dictionary<Color, KingPiece?> Kings { get; }

    #endregion



    #region constructor

    public Board(List<IPiece> pieces = null!)
    {
        State = new IPiece?[BoardSize, BoardSize];
        Kings = new()
        {
            [Color.White] = null,
            [Color.Black] = null
        };

        if (pieces != null)
        {
            foreach (var piece in pieces)
            {
                State[piece.Row, piece.Col] = piece;

                if (piece.PieceType == PieceType.King)
                {
                    Kings[piece.Color] = (KingPiece)piece;
                }
            }
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
        var king = Kings[color];

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
        // THIS IS STILL PRETTY INEFFICIENT CONSIDERING IT WILL BE CALLED FOR EVERY TEST MOVE. REFACTOR TO A PROPERTY LATER WHICH HAS PIECES REMOVED WHEN TAKEN??
        List<IPiece> pieces = [];

        for (int i = MinIndex; i <= MaxIndex; i++)
        {
            for (int j = MinIndex; j <= MaxIndex; j++)
            {
                if (State[i, j] != null && State[i, j]?.Color == color)
                    pieces.Add(State[i, j]!);
            }
        }

        return pieces;
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

        // record the result
        bool result = UnderCheck(movingPiece.Color);

        // roll back pieces
        State[from.row, from.col] = movingPiece;
        movingPiece.Row = from.row;
        movingPiece.Col = from.col;
        State[to.row, to.col] = targetPiece;

        
        return result;
    }

    #endregion
}