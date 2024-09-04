using GameLogic.Enums;

namespace GameLogic.Helpers;

public static class ColorHelpers
{
    /// <summary>
    /// Returns Black if White and White if Black
    /// </summary>
    /// <param name="color"></param>
    /// <returns>A Color Enum value</returns>
    public static PieceColor OppositeColor(PieceColor color)
    {
        if (color == PieceColor.White)
            return PieceColor.Black;
        else
            return PieceColor.White;
    }
}