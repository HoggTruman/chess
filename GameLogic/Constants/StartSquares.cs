namespace GameLogic.Constants;

/// <summary>
/// A static class containing the starting squares for each piece. 
/// A Q denotes Queenside and K denotes Kingside.
/// Pawns are labeled by column (file) letter.
/// </summary>
public static class StartSquares
{
    // White Pieces
    public static (int row, int col) WhiteRookQ { get => (0, 0);}
    public static (int row, int col) WhiteKnightQ { get => (0, 1);}
    public static (int row, int col) WhiteBishopQ { get => (0, 2);}
    public static (int row, int col) WhiteQueen { get => (0, 3);}
    public static (int row, int col) WhiteKing { get => (0, 4);}
    public static (int row, int col) WhiteBishopK { get => (0, 5);}
    public static (int row, int col) WhiteKnightK { get => (0, 6);}
    public static (int row, int col) WhiteRookK { get => (0, 7);}

    public static (int row, int col) WhitePawnA { get => (1, 0);}
    public static (int row, int col) WhitePawnB { get => (1, 1);}
    public static (int row, int col) WhitePawnC { get => (1, 2);}
    public static (int row, int col) WhitePawnD { get => (1, 3);}
    public static (int row, int col) WhitePawnE { get => (1, 4);}
    public static (int row, int col) WhitePawnF { get => (1, 5);}
    public static (int row, int col) WhitePawnG { get => (1, 6);}
    public static (int row, int col) WhitePawnH { get => (1, 7);}

    // Black Pieces
    public static (int row, int col) BlackRookQ { get => (7, 0);}
    public static (int row, int col) BlackKnightQ { get => (7, 1);}
    public static (int row, int col) BlackBishopQ { get => (7, 2);}
    public static (int row, int col) BlackQueen { get => (7, 3);}
    public static (int row, int col) BlackKing { get => (7, 4);}
    public static (int row, int col) BlackBishopK { get => (7, 5);}
    public static (int row, int col) BlackKnightK { get => (7, 6);}
    public static (int row, int col) BlackRookK { get => (7, 7);}

    public static (int row, int col) BlackPawnA { get => (6, 0);}
    public static (int row, int col) BlackPawnB { get => (6, 1);}
    public static (int row, int col) BlackPawnC { get => (6, 2);}
    public static (int row, int col) BlackPawnD { get => (6, 3);}
    public static (int row, int col) BlackPawnE { get => (6, 4);}
    public static (int row, int col) BlackPawnF { get => (6, 5);}
    public static (int row, int col) BlackPawnG { get => (6, 6);}
    public static (int row, int col) BlackPawnH { get => (6, 7);}
}