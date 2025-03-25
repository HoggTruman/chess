using BetterGameLogic.Enums;

namespace BetterGameLogic.Helpers;

public static class ColorHelpers
{
    /// <summary>
    /// Returns the opposite PieceColor
    /// </summary>
    /// <param name="color"></param>
    /// <returns>A PieceColor</returns>
    public static PieceColor Opposite(PieceColor color)
    {
        return color switch
        {
            PieceColor.White => PieceColor.Black,
            PieceColor.Black => PieceColor.White,
            _ => PieceColor.None
        };
    }
}