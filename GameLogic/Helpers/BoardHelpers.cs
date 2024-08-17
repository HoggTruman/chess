using GameLogic.Enums;
using GameLogic.Interfaces;

namespace GameLogic.Helpers;

public static class BoardHelpers
{
    /// <summary>
    /// Creates a new 2D Array which references the same pieces as the state passed in
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public static IPiece?[,] CopyState(IPiece?[,] state)
    {
        IPiece?[,] copy = new IPiece?[Board.BoardSize, Board.BoardSize];

        for (int rowIndex = 0; rowIndex <= Board.MaxIndex; rowIndex++)
        {
            for (int colIndex = 0; colIndex <= Board.MaxIndex; colIndex++)
            {
                copy[rowIndex, colIndex] = state[rowIndex, colIndex];
            }
        }

        return copy;
    }
}