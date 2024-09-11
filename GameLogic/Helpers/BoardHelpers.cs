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
    public static bool SquareIsInBounds((int row, int col) square)
    {
        return (
            square.row >= Board.MinIndex &&
            square.row <= Board.MaxIndex &&
            square.col >= Board.MinIndex &&
            square.col <= Board.MaxIndex
        );
    }


    /// <summary>
    /// Returns the square rotated 180 degrees.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>(row, col) of the rotated square.</returns>
    public static (int row, int col) RotateSquare180((int row, int col) s)
    {
        return (Board.MaxIndex - s.row, Board.MaxIndex - s.col);
    }
}