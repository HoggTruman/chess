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


    /// <summary>
    /// Filters a list of squares to keep those which are in bounds on a board
    /// </summary>
    /// <param name="squares"></param>
    /// <returns></returns>
    public static List<(int row, int col)> KeepInBoundsSquares(List<(int row, int col)> squares)
    {
        return squares.Where(p => 
            p.row >= Board.MinIndex &&
            p.row <= Board.MaxIndex &&
            p.col >= Board.MinIndex &&
            p.col <= Board.MaxIndex
        ).ToList();
    }
}