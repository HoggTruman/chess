using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;
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

    public List<IMove> MoveHistory { get; } = [];

    #endregion



    #region constructor

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pieces"></param>
    public Board()
    {
        State = new IPiece?[BoardSize, BoardSize];
        Kings = new()
        {
            [Color.White] = null,
            [Color.Black] = null
        };
    }

    #endregion



    #region public methods

    public void AddPieces(List<IPiece> pieces)
    {
        // add additional checking for no overlapping pieces, pieces already present, two kings etc...

        foreach (var piece in pieces)
        {
            State[piece.Row, piece.Col] = piece;

            if (piece.PieceType == PieceType.King)
            {
                Kings[piece.Color] = (KingPiece)piece;
            }
        }
    }


    public List<(int row, int col)> GetValidMoves(IPiece piece) 
    {
        List<(int row, int col)> validMoves = [];

        // get all reachable squares
        List<(int row, int col)> allMoves = piece.GetReachableSquares(this);

        // keep the squares which represent a valid move
        var king = Kings[piece.Color];

        foreach (var move in allMoves)
        {
            if (MoveLeavesPlayerInCheck(piece.Square, move, king) == false)
            {
                validMoves.Add(move);
            }
        }

        return validMoves;
    }

    public void HandleMove(IMove move)
    {
        MoveHistory.Add(move);

        if (move.MoveType == MoveType.Move)
        {
            MovePiece((Move)move);
        }
        else if (move.MoveType == MoveType.EnPassant)
        {
            EnPassant((EnPassantMove)move);
        }
        // add castle + promotion handling
    }


    public List<IPiece> GetPiecesByColor(Color color)
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

    #endregion



    #region private methods

    private bool MoveLeavesPlayerInCheck((int row, int col) from, (int row, int col) to, KingPiece? king)
    {
        // Return false when no king. Makes testing more convenient for when no king is present
        if (king == null)
            return false;

        IPiece? movingPiece = State[from.row, from.col];
        IPiece? targetPiece = State[to.row, to.col]; // DOESNT WORK FOR EN PASSANT

        if (movingPiece == null)
            throw new ArgumentException("Piece not found at specified from square");
    

        // make the move
        State[from.row, from.col] = null;
        State[to.row, to.col] = movingPiece;
        movingPiece.Row = to.row;
        movingPiece.Col = to.col;

        // record the result
        bool result = king.IsChecked(this);

        // roll back pieces
        State[from.row, from.col] = movingPiece;
        movingPiece.Row = from.row;
        movingPiece.Col = from.col;
        State[to.row, to.col] = targetPiece;

        
        return result;
    }


    /// <summary>
    /// Updates the State of the board to reflect the move. 
    /// Updates the Row and Col properties of the moving piece
    /// </summary>
    /// <param name="move"></param>
    private void MovePiece(IMove move)
    {
        var movingPiece = State[move.From.row, move.From.col];

        if (movingPiece == null)
            throw new ArgumentException("No piece at from location");

        // Update State for the movingPiece
        State[move.To.row, move.To.col] = movingPiece;
        State[move.From.row, move.From.col] = null;
        
        // Update the movingPiece row and col
        movingPiece.Row = move.To.row;
        movingPiece.Col = move.To.col;
    }


    /// <summary>
    /// Updates the State of the board to reflect the En Passant move. 
    /// Updates the Row and Col properties of the moving pawn.
    /// </summary>
    /// <param name="move"></param>
    private void EnPassant(EnPassantMove move)
    {
        // Move the pawn
        MovePiece(move);

        // Remove captured pawn from the board
        State[move.Captured.row, move.Captured.col] = null;
    }


    private void PawnPromote((int row, int col) from, (int row, int col) to, PieceType promoteTo)
    {
        throw new NotImplementedException();
    }


    // king location may not be needed since king always castles from same square
    private void Castle((int row, int col) kingFrom, (int row, int col) rookFrom)
    {
        throw new NotImplementedException();
    }

    #endregion
}