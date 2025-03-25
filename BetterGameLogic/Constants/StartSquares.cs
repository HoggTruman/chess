namespace BetterGameLogic.Constants;

/// <summary>
/// A static class containing the starting squares for each piece. 
/// A Q denotes Queenside and K denotes Kingside.
/// Pawns are labeled by column (file) letter.
/// </summary>
public static class StartSquares
{
    // White Pieces
    public static readonly Square WhiteRookQ = new() { Row = 7, Col = 0 };
    public static readonly Square WhiteKnightQ = new() { Row = 7, Col = 1 };
    public static readonly Square WhiteBishopQ = new() { Row = 7, Col = 2};
    public static readonly Square WhiteQueen = new() { Row = 7, Col = 3};
    public static readonly Square WhiteKing = new() { Row = 7, Col = 4};
    public static readonly Square WhiteBishopK = new() { Row = 7, Col = 5};
    public static readonly Square WhiteKnightK = new() { Row = 7, Col = 6};
    public static readonly Square WhiteRookK = new() { Row = 7, Col = 7};

    public static readonly Square WhitePawnA = new() { Row = 6, Col = 0};
    public static readonly Square WhitePawnB = new() { Row = 6, Col = 1};
    public static readonly Square WhitePawnC = new() { Row = 6, Col = 2};
    public static readonly Square WhitePawnD = new() { Row = 6, Col = 3};
    public static readonly Square WhitePawnE = new() { Row = 6, Col = 4};
    public static readonly Square WhitePawnF = new() { Row = 6, Col = 5};
    public static readonly Square WhitePawnG = new() { Row = 6, Col = 6};
    public static readonly Square WhitePawnH = new() { Row = 6, Col = 7};

    // Black Pieces
    public static readonly Square BlackRookQ = new() { Row = 0, Col = 0};
    public static readonly Square BlackKnightQ = new() { Row = 0, Col = 1};
    public static readonly Square BlackBishopQ = new() { Row = 0, Col = 2};
    public static readonly Square BlackQueen = new() { Row = 0, Col = 3};
    public static readonly Square BlackKing = new() { Row = 0, Col = 4};
    public static readonly Square BlackBishopK = new() { Row = 0, Col = 5};
    public static readonly Square BlackKnightK = new() { Row = 0, Col = 6};
    public static readonly Square BlackRookK = new() { Row = 0, Col = 7};

    public static readonly Square BlackPawnA = new() { Row = 1, Col = 0};
    public static readonly Square BlackPawnB = new() { Row = 1, Col = 1};
    public static readonly Square BlackPawnC = new() { Row = 1, Col = 2};
    public static readonly Square BlackPawnD = new() { Row = 1, Col = 3};
    public static readonly Square BlackPawnE = new() { Row = 1, Col = 4};
    public static readonly Square BlackPawnF = new() { Row = 1, Col = 5};
    public static readonly Square BlackPawnG = new() { Row = 1, Col = 6};
    public static readonly Square BlackPawnH = new() { Row = 1, Col = s7};
}
