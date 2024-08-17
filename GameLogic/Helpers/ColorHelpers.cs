using GameLogic.Enums;

namespace GameLogic.Helpers;

public static class ColorHelpers
{
    /// <summary>
    /// Returns Black if White and White if Black
    /// </summary>
    /// <param name="color"></param>
    /// <returns>A Color Enum value</returns>
    public static Color OppositeColor(Color color)
    {
        if (color == Color.White)
            return Color.Black;
        else
            return Color.White;
    }
}