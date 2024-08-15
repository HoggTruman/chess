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
    public static List<(int row, int col)> ScanRowAndCol(int pieceRow, int pieceCol, Board board)
    {
        List<(int row, int col)> targetedSquares = [];

        // Scan lower index columns of row
        for (int colIndex = pieceCol - 1; colIndex >= Board.MinIndex; colIndex--)
        {
            targetedSquares.Add((row: pieceRow, col: colIndex));

            if (board.State[pieceRow, colIndex] != null)
            {
                break;  
            }
        }

        // Scan higher index columns of row
        for (int colIndex = pieceCol + 1; colIndex <= Board.MaxIndex; colIndex++)
        {
            targetedSquares.Add((row: pieceRow, col: colIndex));
            
            if (board.State[pieceRow, colIndex] != null)
            {
                break;
            }
        }

        // Scan lower index rows of column
        for (int rowIndex = pieceRow - 1; rowIndex >= Board.MinIndex; rowIndex--)
        {
            targetedSquares.Add((row: rowIndex, col: pieceCol));

            if (board.State[rowIndex, pieceCol] != null)
            {
                break;
            }  
        }

        // Scan higher index rows of column
        for (int rowIndex = pieceRow + 1; rowIndex <= Board.MaxIndex; rowIndex++)
        {
            targetedSquares.Add((row: rowIndex, col: pieceCol));

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
    public static List<(int row, int col)> ScanDiagonals(int pieceRow, int pieceCol, Board board)
    {
        List<(int row, int col)> targetedSquares = [];

        // scan diagonal with lower index row and lower index col
        for 
        (
            int rowIndex = pieceRow - 1, colIndex = pieceCol - 1; 
            rowIndex >= Board.MinIndex && colIndex >= Board.MinIndex;
            rowIndex--, colIndex--
        )
        {
            targetedSquares.Add((row: rowIndex, col: colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }

        // scan diagonal with lower index row and higher index col
        for (
            int rowIndex = pieceRow - 1, colIndex = pieceCol + 1; 
            rowIndex >= Board.MinIndex && colIndex <= Board.MaxIndex;
            rowIndex--, colIndex++
        )
        {
            targetedSquares.Add((row: rowIndex, col: colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }

        // scan diagonal with higher index row and higher index col
        for (
            int rowIndex = pieceRow + 1, colIndex = pieceCol + 1; 
            rowIndex <= Board.MaxIndex && colIndex <= Board.MaxIndex;
            rowIndex++, colIndex++
        )
        {
            targetedSquares.Add((row: rowIndex, col: colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }

        // scan diagonal with higher index row and lower index col
        for (
            int rowIndex = pieceRow + 1, colIndex = pieceCol - 1; 
            rowIndex <= Board.MaxIndex && colIndex >= Board.MinIndex;
            rowIndex++, colIndex--
        )
        {
            targetedSquares.Add((row: rowIndex, col: colIndex));

            if (board.State[rowIndex, colIndex] != null)
            {
                break;
            }
        }


        return targetedSquares;
    }
}