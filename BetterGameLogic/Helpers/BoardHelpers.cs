namespace BetterGameLogic.Helpers;

public class BoardHelpers
{
    /// <summary>
    /// Returns the square rotated 180 degrees.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Square representing the rotated square.</returns>
    public static Square RotateSquare180(Square s)
    {
        return new(
            Board.BoardSize - s.Row - 1, 
            Board.BoardSize - s.Col - 1);
    }
}
