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

    /// <summary>
    /// Adds a new piece to the Board of specified PieceType at (row, col).
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="row">The row to place the new piece at.</param>
    /// <param name="col">The column to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new piece of type T</returns>
    /// <exception cref="ArgumentException"></exception>
    public T AddNewPiece<T>(int row, int col, Color color=Color.White) where T : Piece
    {
        // Ensure the square is empty before creating a piece there
        if (State[row, col] != null)
        {
            throw new ArgumentException($"Board already contains a piece at (row: {row}, column: {col})");
        }

        // Create a piece of type T
        T piece = (T)(Activator.CreateInstance(typeof(T), this, row, col, color) ?? throw new Exception());

        // Handle king case
        if (typeof(T) == typeof(KingPiece))
        {
            if (Kings[color] != null)
            {   
                throw new ArgumentException($"{color} King already exists on the board");
            }

            Kings[color] = piece as KingPiece;
        }
        
        // Update the board state with the piece
        State[row, col] = piece;
            
        return piece;
    }


    /// <summary>
    /// Adds a new piece to the Board of specified PieceType at the provided square.
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="pieceType">The PieceType of the new piece.</param>
    /// <param name="square">The (row, column) square to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new IPiece</returns>
    public T AddNewPiece<T>((int row, int col) square, Color color = Color.White) where T : Piece
    {
        return AddNewPiece<T>(square.row, square.col, color);
    }


    public void HandleMove(IMove move)
    {
        MoveHistory.Add(move);

        if (move.MoveType == MoveType.Standard)
        {
            MovePiece((StandardMove)move);
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


    /// <summary>
    /// Returns a bool of whether the player is left in check by making the move. 
    /// For En Passant, pass in the captured pawn's square as the "captured" parameter.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="captured"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool MoveLeavesPlayerInCheck((int row, int col) from, (int row, int col) to, (int row, int col)? captured = null)
    {
        // Ensure there the moving piece is not null
        IPiece? movingPiece = State[from.row, from.col];

        if (movingPiece == null)
            throw new ArgumentException("Piece not found at specified from square");

        // Return false when no king. Makes testing more convenient for when no king is present
        KingPiece? king = Kings[movingPiece.Color];
        if (king == null)
            return false;

        // Get target piece
        (int row, int col) capturedSquare = to;

        if (captured != null)
            capturedSquare = ((int row, int col))captured;

        IPiece? capturedPiece = State[capturedSquare.row, capturedSquare.col];

        // make the move
        State[from.row, from.col] = null;
        State[to.row, to.col] = movingPiece;
        movingPiece.Square = to;

        // record the result
        bool result = king.IsChecked();

        // roll back pieces
        State[from.row, from.col] = movingPiece;
        movingPiece.Row = from.row;
        movingPiece.Col = from.col;
        State[capturedSquare.row, capturedSquare.col] = capturedPiece;

        
        return result;
    }

    #endregion



    #region private methods

    /// <summary>
    /// Updates the State of the board to reflect the move. 
    /// Updates the Row and Col properties of the moving piece
    /// </summary>
    /// <param name="move"></param>
    private void MovePiece(SinglePieceMove move)
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


    private void PawnPromote(PromotionMove move)
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