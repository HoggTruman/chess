using GameLogic;
using GameLogic.Enums;
using GameLogic.Pieces;
using FluentAssertions;

namespace GameLogicTests.Pieces;

public class QueenPieceTests
{
    #region GetTargetedSquares Tests
    
    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var queen = new QueenPiece(board, 4, 4, PieceColor.White);
        var rowBlockingPiece = new QueenPiece(board, 4, 5, PieceColor.White);
        var colBlockingPiece = new QueenPiece(board, 5, 4, PieceColor.White);
        var diagonalBlockingPiece = new QueenPiece(board, 5, 5, PieceColor.White);

        board.AddPiece(queen);
        board.AddPiece(rowBlockingPiece);
        board.AddPiece(colBlockingPiece);
        board.AddPiece(diagonalBlockingPiece);

        List<Square> expected = [
            // row squares
            new(4, 0),
            new(4, 1),
            new(4, 2),
            new(4, 3),
            new(4, 5),
            // column squares
            new(1, 4),
            new(2, 4),
            new(3, 4),
            new(0, 4),
            new(5, 4),
            // positive diagonal squares
            new(0, 0),
            new(1, 1),
            new(2, 2),
            new(3, 3),
            new(5, 5),
            // negative diagonal squares
            new(3, 5),
            new(2, 6),
            new(1, 7),
            new(5, 3),
            new(6, 2),
            new(7, 1)    
        ];

        // Act
        var result = queen.GetTargetedSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    #endregion



    #region GetReachableSquares Tests

    [Fact]
    public void GetReachableSquares_ReturnsReachableSquares()
    {
        // Arrange
        Board board = new();

        var queen = new QueenPiece(board, 4, 4, PieceColor.White);
        var rowBlockingPiece = new QueenPiece(board, 4, 5, PieceColor.White);
        var colBlockingPiece = new QueenPiece(board, 5, 4, PieceColor.White);
        var diagonalBlockingPiece = new QueenPiece(board, 5, 5, PieceColor.White);
        var enemyBlockingPiece = new QueenPiece(board, 3, 5, PieceColor.Black);

        board.AddPiece(queen);
        board.AddPiece(rowBlockingPiece);
        board.AddPiece(colBlockingPiece);
        board.AddPiece(diagonalBlockingPiece);
        board.AddPiece(enemyBlockingPiece);

        List<Square> expected = [
            // row squares
            new(4, 0),
            new(4, 1),
            new(4, 2),
            new(4, 3),
            // column squares
            new(1, 4),
            new(2, 4),
            new(3, 4),
            new(0, 4),
            // positive diagonal squares
            new(0, 0),
            new(1, 1),
            new(2, 2),
            new(3, 3),
            // negative diagonal squares
            new(3, 5),
            new(5, 3),
            new(6, 2),
            new(7, 1)    
        ];

        // Act
        var result = queen.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion
}