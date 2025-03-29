using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogic;

public class Board
{
    public const int BoardSize = 8;

    public IPiece?[,] State { get; private set; } = new IPiece?[BoardSize, BoardSize];

    public Dictionary<PieceColor, List<IPiece>> Pieces { get; } = new()
    {
        [PieceColor.White] = [],
        [PieceColor.Black] = [],
    };


    private readonly Dictionary<PieceColor, KingPiece?> _kingPieces = new()
    {
        [PieceColor.White] = null,
        [PieceColor.Black] = null
    };

    public History History = new();

    #region Methods

    public void Initialize()
    {
        // Ensure the board is empty
        if (Pieces[PieceColor.White].Count != 0 || Pieces[PieceColor.Black].Count != 0)
        {
            throw new Exception("Board must be empty to be Initialized.");
        }

        // Add White pieces
        AddPiece(new RookPiece(this, StartSquares.WhiteRookQ, PieceColor.White));
        AddPiece(new KnightPiece(this, StartSquares.WhiteKnightQ, PieceColor.White));
        AddPiece(new BishopPiece(this, StartSquares.WhiteBishopQ, PieceColor.White));
        AddPiece(new QueenPiece(this, StartSquares.WhiteQueen, PieceColor.White));
        AddPiece(new KingPiece(this, StartSquares.WhiteKing, PieceColor.White));
        AddPiece(new BishopPiece(this, StartSquares.WhiteBishopK, PieceColor.White));
        AddPiece(new KnightPiece(this, StartSquares.WhiteKnightK, PieceColor.White));
        AddPiece(new RookPiece(this, StartSquares.WhiteRookK, PieceColor.White));

        AddPiece(new PawnPiece(this, StartSquares.WhitePawnA, PieceColor.White));
        AddPiece(new PawnPiece(this, StartSquares.WhitePawnB, PieceColor.White));
        AddPiece(new PawnPiece(this, StartSquares.WhitePawnC, PieceColor.White));
        AddPiece(new PawnPiece(this, StartSquares.WhitePawnD, PieceColor.White));
        AddPiece(new PawnPiece(this, StartSquares.WhitePawnE, PieceColor.White));
        AddPiece(new PawnPiece(this, StartSquares.WhitePawnF, PieceColor.White));
        AddPiece(new PawnPiece(this, StartSquares.WhitePawnG, PieceColor.White));
        AddPiece(new PawnPiece(this, StartSquares.WhitePawnH, PieceColor.White));

        // Add Black pieces
        AddPiece(new RookPiece(this, StartSquares.BlackRookQ, PieceColor.Black));
        AddPiece(new KnightPiece(this, StartSquares.BlackKnightQ, PieceColor.Black));
        AddPiece(new BishopPiece(this, StartSquares.BlackBishopQ, PieceColor.Black));
        AddPiece(new QueenPiece(this, StartSquares.BlackQueen, PieceColor.Black));
        AddPiece(new KingPiece(this, StartSquares.BlackKing, PieceColor.Black));
        AddPiece(new BishopPiece(this, StartSquares.BlackBishopK, PieceColor.Black));
        AddPiece(new KnightPiece(this, StartSquares.BlackKnightK, PieceColor.Black));
        AddPiece(new RookPiece(this, StartSquares.BlackRookK, PieceColor.Black));

        AddPiece(new PawnPiece(this, StartSquares.BlackPawnA, PieceColor.Black));
        AddPiece(new PawnPiece(this, StartSquares.BlackPawnB, PieceColor.Black));
        AddPiece(new PawnPiece(this, StartSquares.BlackPawnC, PieceColor.Black));
        AddPiece(new PawnPiece(this, StartSquares.BlackPawnD, PieceColor.Black));
        AddPiece(new PawnPiece(this, StartSquares.BlackPawnE, PieceColor.Black));
        AddPiece(new PawnPiece(this, StartSquares.BlackPawnF, PieceColor.Black));
        AddPiece(new PawnPiece(this, StartSquares.BlackPawnG, PieceColor.Black));
        AddPiece(new PawnPiece(this, StartSquares.BlackPawnH, PieceColor.Black));
    }


    public void AddPiece(IPiece piece)
    {
        if (State[piece.Row, piece.Col] != null)
        {
            throw new ArgumentException("A piece can not be added to an occupied square");
        }

        if (piece is KingPiece &&
            _kingPieces[piece.Color] != null)
        {
            throw new ArgumentException("Only one king of each color can exist on the board");
        }

        State[piece.Row, piece.Col] = piece;
        Pieces[piece.Color].Add(piece);

        if (piece is KingPiece kingPiece)
        {
            _kingPieces[piece.Color] = kingPiece;
        }
    }

    /// <summary>
    /// Moves the piece on the "from" square to the "to" square.
    /// Does not update history
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <exception cref="ArgumentException"></exception>
    public void MovePiece(Square from, Square to)
    {
        var movingPiece = State[from.Row, from.Col];

        if (movingPiece == null)
        {
            throw new ArgumentException($"A piece does not exist on the board at (row:{from.Row}, col: {from.Col})");
        }

        // Update State for the movingPiece
        State[to.Row, to.Col] = movingPiece;
        State[from.Row, from.Col] = null;
        
        // Update the movingPiece row and col
        movingPiece.Square = to;
    }


    public void RemoveAt(Square square)
    {
        IPiece? piece = At(square);
        if (piece == null)
        {
            return;
        }

        State[square.Row, square.Col] = null;
        Pieces[piece.Color].Remove(piece);

        if (piece is KingPiece)
        {
            _kingPieces[piece.Color] = null;
        }
    }


    public KingPiece GetKing(PieceColor color)
    {
        return _kingPieces[color] ?? throw new Exception($"Board does not contain a {color} king");
    }


    public IPiece? At(Square square)
    {
        return State[square.Row, square.Col];
    }


    public static bool IsInBounds(Square square)
    {
        return IsInBounds(square.Row, square.Col);
    }

    public static bool IsInBounds(int row, int col)
    {
        return row >= 0 &&
               row < BoardSize &&
               col >= 0 &&
               col < BoardSize;
    }


    public bool IsOccupiedByColor(Square square, PieceColor color)
    {
        return IsOccupiedByColor(square.Row, square.Col, color);
    }

    public bool IsOccupiedByColor(int row, int col, PieceColor color)
    {
        return State[row, col] != null && 
               State[row, col]!.Color == color;
    }

    #endregion
}
