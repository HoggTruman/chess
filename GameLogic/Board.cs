using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Helpers;
using GameLogic.Interfaces;
using GameLogic.Moves;
using GameLogic.Pieces;



namespace GameLogic;

public class Board
{
    #region Constants

    public const int BoardSize = 8;
    public const int MinIndex = 0;
    public const int MaxIndex = BoardSize - 1;

    #endregion



    #region Properties

    public IPiece?[,] State { get; set; }

    public Dictionary<Color, KingPiece?> Kings { get; }

    public Dictionary<Color, List<IPiece>> Pieces { get; }

    public List<IMove> MoveHistory { get; } = [];

    #endregion



    #region Constructors

    /// <summary>
    /// Initializes a new instance of the Board class.
    /// </summary>
    public Board()
    {
        State = new IPiece?[BoardSize, BoardSize];
        Kings = new()
        {
            [Color.White] = null,
            [Color.Black] = null
        };
        Pieces = new()
        {
            [Color.White] = [],
            [Color.Black] = [],
        };
    }

    #endregion



    #region Public Methods

    /// <summary>
    /// Sets up the board with pieces in position for a new game.
    /// The board must be empty to use this method.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Initialize()
    {
        // Ensure the board is empty
        if (Pieces[Color.White].Count != 0 || Pieces[Color.Black].Count != 0)
        {
            throw new Exception("Board must be empty to be Initialized.");
        }

        // Add White pieces
        AddNewPiece<RookPiece>(StartSquares.WhiteRookQ, Color.White);
        AddNewPiece<KnightPiece>(StartSquares.WhiteKnightQ, Color.White);
        AddNewPiece<BishopPiece>(StartSquares.WhiteBishopQ, Color.White);
        AddNewPiece<QueenPiece>(StartSquares.WhiteQueen, Color.White);
        AddNewPiece<KingPiece>(StartSquares.WhiteKing, Color.White);
        AddNewPiece<BishopPiece>(StartSquares.WhiteBishopK, Color.White);
        AddNewPiece<KnightPiece>(StartSquares.WhiteKnightK, Color.White);
        AddNewPiece<RookPiece>(StartSquares.WhiteRookK, Color.White);

        AddNewPiece<PawnPiece>(StartSquares.WhitePawnA, Color.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnB, Color.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnC, Color.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnD, Color.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnE, Color.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnF, Color.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnG, Color.White);
        AddNewPiece<PawnPiece>(StartSquares.WhitePawnH, Color.White);

        // Add Black pieces
        AddNewPiece<RookPiece>(StartSquares.BlackRookQ, Color.Black);
        AddNewPiece<KnightPiece>(StartSquares.BlackKnightQ, Color.Black);
        AddNewPiece<BishopPiece>(StartSquares.BlackBishopQ, Color.Black);
        AddNewPiece<QueenPiece>(StartSquares.BlackQueen, Color.Black);
        AddNewPiece<KingPiece>(StartSquares.BlackKing, Color.Black);
        AddNewPiece<BishopPiece>(StartSquares.BlackBishopK, Color.Black);
        AddNewPiece<KnightPiece>(StartSquares.BlackKnightK, Color.Black);
        AddNewPiece<RookPiece>(StartSquares.BlackRookK, Color.Black);

        AddNewPiece<PawnPiece>(StartSquares.BlackPawnA, Color.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnB, Color.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnC, Color.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnD, Color.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnE, Color.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnF, Color.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnG, Color.Black);
        AddNewPiece<PawnPiece>(StartSquares.BlackPawnH, Color.Black);
    }


    /// <summary>
    /// Adds a new piece to the Board of type <typeparamref name="T"/> at (row, col).
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="row">The row index to place the new piece at.</param>
    /// <param name="col">The column index to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new piece of type <typeparamref name="T"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public T AddNewPiece<T>(int row, int col, Color color=Color.White) where T : Piece
    {
        // Ensure row and col are in-bounds
        if (row < MinIndex || row > MaxIndex ||
            col < MinIndex || col > MaxIndex)
        {
            throw new ArgumentException("Row / column index out of range. " +
                $"Must be between {MinIndex} and {MaxIndex} (inclusive)");
        }

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

        // Update corresponding piece list
        Pieces[color].Add(piece);
        
            
        return piece;
    }


    /// <summary>
    /// Adds a new piece to the Board of type <typeparamref name="T"/> at the provided square.
    /// New pieces should always be created in this way.
    /// </summary>
    /// <typeparam name="T">A child of the Piece abstract class</typeparam>
    /// <param name="square">The (row, column) square to place the new piece at.</param>
    /// <param name="color">The Color of the new piece.</param>
    /// <returns>The new piece of type <typeparamref name="T"/></returns>
    public T AddNewPiece<T>((int row, int col) square, Color color = Color.White) where T : Piece
    {
        return AddNewPiece<T>(square.row, square.col, color);
    }


    // REFACTOR OUT LATER?
    public void HandleMove(IMove move)
    {
        MoveHistory.Add(move);

        if (move.MoveType == MoveType.Standard)
        {
            StandardMove((StandardMove)move);
        }
        else if (move.MoveType == MoveType.EnPassant)
        {
            EnPassant((EnPassantMove)move);
        }
        else if (move.MoveType == MoveType.Promotion)
        {
            PawnPromote((PromotionMove)move);
        }
        else if (move.MoveType == MoveType.Castle)
        {
            Castle((CastleMove)move);
        }
    }


    /// <summary>
    /// Determines if a player will leave themself in check by moving the piece at "from" to "to".
    /// For En Passant, pass in the captured pawn's square as the "captured" parameter.
    /// Castling does its own verification and is not passed through this method.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="captured"></param>
    /// <returns>true if the player would be in check after the move. Otherwise, false</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool MoveLeavesPlayerInCheck((int row, int col) from, (int row, int col) to, (int row, int col)? captured = null)
    {
        // Ensure there the moving piece is not null
        IPiece? movingPiece = State[from.row, from.col];

        if (movingPiece == null)
        {
            throw new ArgumentException($"Piece not found at \"from\": (row: {from.row}, col: {from.col})");
        }
            
        // Return false when no king.
        KingPiece? playerKing = Kings[movingPiece.Color];
        if (playerKing == null)
        {
            return false;
        }

        // Get target piece
        IPiece? capturedPiece = State[to.row, to.col];

        if (captured != null)
        {
            // Set capturedPiece for En Passant
            capturedPiece = State[captured.Value.row, captured.Value.col];

            // Set the "captured" square to null for En Passant
            State[captured.Value.row, captured.Value.col] = null;
        }

        // Make the move
        State[from.row, from.col] = null;
        State[to.row, to.col] = movingPiece;
        movingPiece.Square = to;

        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Remove(capturedPiece);
        }

        // Record the result
        bool result = playerKing.IsChecked();

        // Roll back pieces
        State[from.row, from.col] = movingPiece;
        movingPiece.Square = from;
        
        if (captured != null)
        {
            // En Passant revert
            State[to.row, to.col] = null;
            State[captured.Value.row, captured.Value.col] = capturedPiece;
        }
        else
        {
            State[to.row, to.col] = capturedPiece;
        }

        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Add(capturedPiece);
        }

        
        return result;
    }


    /// <summary>
    /// Updates the Board and involved pieces to reflect the StandardMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">A StandardMove instance</param>
    public void StandardMove(StandardMove move)
    {
        // Remove captured piece from Pieces
        var capturedPiece = State[move.To.row, move.To.col];
        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Remove(capturedPiece);
            State[move.To.row, move.To.col] = null;
        }

        // Move the piece 
        MovePiece(move.From, move.To);
    }


    /// <summary>
    /// Updates the Board and involved pieces to reflect the EnPassantMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">An EnPassantMove instance</param>
    public void EnPassant(EnPassantMove move)
    {
        // Remove captured pawn from the board
        var capturedPawn = State[move.Captured.row, move.Captured.col];
        if (capturedPawn != null)
        {
            Pieces[capturedPawn.Color].Remove(capturedPawn);
            State[move.Captured.row, move.Captured.col] = null;
        }

        // Move the pawn
        MovePiece(move.From, move.To);
    }


    /// <summary>
    /// Updates the Board and involved pieces to reflect the PromotionMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">A PromotionMove instance</param>
    /// <exception cref="ArgumentException"></exception>
    public void PawnPromote(PromotionMove move)
    {
        // Remove pawn
        var pawn = State[move.From.row, move.From.col];
        if (pawn == null)
        {
            throw new ArgumentException($"Piece not found at: (row: {move.From.row}, col:{move.From.col})");
        }
        Pieces[pawn.Color].Remove(pawn);
        State[move.From.row, move.From.col] = null;

        // Remove captured piece
        var capturedPiece = State[move.To.row, move.To.col];
        if (capturedPiece != null)
        {
            Pieces[capturedPiece.Color].Remove(capturedPiece);
            State[move.To.row, move.To.col] = null;
        }

        // Create promoted piece
        if (move.PromotedTo == PieceType.Queen)
        {
            AddNewPiece<QueenPiece>(move.To, pawn.Color);
        }
        else if (move.PromotedTo == PieceType.Rook)
        {
            AddNewPiece<RookPiece>(move.To, pawn.Color);
        }
        else if (move.PromotedTo == PieceType.Knight)
        {
            AddNewPiece<KnightPiece>(move.To, pawn.Color);
        }
        else if (move.PromotedTo == PieceType.Bishop)
        {
            AddNewPiece<BishopPiece>(move.To, pawn.Color);
        }
        else
        {
            throw new ArgumentException(@$"{move.PromotedTo} is not a valid promotion option." + 
                " A pawn can only be promoted to a queen, rook, knight or bishop.");
        }
    }


    /// <summary>
    /// Updates the Board and the involved king and rook to reflect the CastleMove.
    /// The move is assumed to be valid.
    /// </summary>
    /// <param name="move">A CastleMove instance</param>
    public void Castle(CastleMove move)
    {
        // Move king
        MovePiece(move.KingFrom, move.KingTo);

        // Move Rook
        MovePiece(move.RookFrom, move.RookTo); 
    }

    #endregion



    #region Private Methods

    /// <summary>
    /// Updates the <see cref="State"/> to reflect the move. 
    /// Updates the Row and Col properties of the moving piece.
    /// </summary>
    /// <param name="from">The square the piece is moving from</param>
    /// <param name="to">The square the piece is moving to</param>
    /// <exception cref="ArgumentException"></exception>
    private void MovePiece((int row, int col) from, (int row, int col) to)
    {
        var movingPiece = State[from.row, from.col];

        if (movingPiece == null)
        {
            throw new ArgumentException($"A piece does not exist on the board at (row:{from.row}, col: {from.col})");
        }

        // Update State for the movingPiece
        State[to.row, to.col] = movingPiece;
        State[from.row, from.col] = null;
        
        // Update the movingPiece row and col
        movingPiece.Square = to;
    }

    #endregion
}