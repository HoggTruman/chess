namespace GameLogic.Helpers;

public static class PieceHelpers
{
    /// <summary>
    /// Finds the squares a Rook or Queen threatens with a capture on its row and column
    /// </summary>
    /// <param name="pieceRow"></param>
    /// <param name="pieceCol"></param>
    /// <param name="board"></param>
    /// <returns>List of (row, col) tuples</returns>
    public static List<Square> GetTargetedRowColSquares(int pieceRow, int pieceCol, Board board)
    {
        List<Square> targetedSquares = [];

        // Scan lower index columns of row
        for (int colIndex = pieceCol - 1; colIndex >= 0; --colIndex)
        {
            targetedSquares.Add(new(pieceRow, colIndex));

            if (board.State[pieceRow, colIndex] != null)
            {
                break;  
            }
        }

        // Scan higher index columns of row
        for (int colIndex = pieceCol + 1; colIndex < Board.BoardSize; ++colIndex)
        {
            targetedSquares.Add(new(pieceRow, colIndex));
            
            if (board.State[pieceRow, colIndex] != null)
            {
                break;
            }
        }

        // Scan lower index rows of column
        for (int rowIndex = pieceRow - 1; rowIndex >= 0; --rowIndex)
        {
            targetedSquares.Add(new(rowIndex, pieceCol));

            if (board.State[rowIndex, pieceCol] != null)
            {
                break;
            }  
        }

        // Scan higher index rows of column
        for (int rowIndex = pieceRow + 1; rowIndex < Board.BoardSize; ++rowIndex)
        {
            targetedSquares.Add(new(rowIndex, pieceCol));

            if (board.State[rowIndex, pieceCol] != null)
            {
                break;
            }
        }

        return targetedSquares;
    }

    /// <summary>
    /// Finds the squares a Bishop or Queen threatens with a capture on its diagonals
    /// </summary>
    /// <param name="pieceRow"></param>
    /// <param name="pieceCol"></param>
    /// <param name="board"></param>
    /// <returns>List of (row, col) tuples</returns>
    public static List<Square> GetTargetedDiagonalSquares(int pieceRow, int pieceCol, Board board)
    {
        List<Square> targetedSquares = [];

        // Scan diagonal with lower index row and lower index col
        for (int rowIndex = pieceRow - 1, colIndex = pieceCol - 1; 
             rowIndex >= 0 && colIndex >= 0;
             --rowIndex, --colIndex)
        {
            targetedSquares.Add(new(rowIndex, colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }

        // Scan diagonal with lower index row and higher index col
        for (int rowIndex = pieceRow - 1, colIndex = pieceCol + 1; 
             rowIndex >= 0 && colIndex < Board.BoardSize;
             --rowIndex, ++colIndex)
        {
            targetedSquares.Add(new(rowIndex, colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }

        // Scan diagonal with higher index row and higher index col
        for (int rowIndex = pieceRow + 1, colIndex = pieceCol + 1; 
             rowIndex < Board.BoardSize && colIndex < Board.BoardSize;
             ++rowIndex, ++colIndex)
        {
            targetedSquares.Add(new(rowIndex, colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }

        // Scan diagonal with higher index row and lower index col
        for (int rowIndex = pieceRow + 1, colIndex = pieceCol - 1; 
             rowIndex < Board.BoardSize && colIndex >= 0;
             ++rowIndex, --colIndex
        )
        {
            targetedSquares.Add(new(rowIndex, colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }

        return targetedSquares;
    }
}
