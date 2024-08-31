using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class QueenPieceTests
{
    #region GetTargetedSquares Tests
    
    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var queen = board.AddNewPiece<QueenPiece>(4, 4, Color.White);

        var rowBlockingPiece = board.AddNewPiece<QueenPiece>(4, 5, Color.White);
        var colBlockingPiece = board.AddNewPiece<QueenPiece>(5, 4, Color.White);
        var diagonalBlockingPiece = board.AddNewPiece<QueenPiece>(5, 5, Color.White);

        List<(int row, int col)> expected = [
            // row squares
            (4, 0),
            (4, 1),
            (4, 2),
            (4, 3),
            (4, 5),
            // column squares
            (1, 4),
            (2, 4),
            (3, 4),
            (0, 4),
            (5, 4),
            // positive diagonal squares
            (0, 0),
            (1, 1),
            (2, 2),
            (3, 3),
            (5, 5),
            // negative diagonal squares
            (3, 5),
            (2, 6),
            (1, 7),
            (5, 3),
            (6, 2),
            (7, 1)    
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

        var queen = board.AddNewPiece<QueenPiece>(4, 4, Color.White);

        var rowBlockingPiece = board.AddNewPiece<QueenPiece>(4, 5, Color.White);
        var colBlockingPiece = board.AddNewPiece<QueenPiece>(5, 4, Color.White);
        var diagonalBlockingPiece = board.AddNewPiece<QueenPiece>(5, 5, Color.White);
        var enemyBlockingPiece = board.AddNewPiece<QueenPiece>(3, 5, Color.Black);

        List<(int row, int col)> expected = [
            // row squares
            (4, 0),
            (4, 1),
            (4, 2),
            (4, 3),
            // column squares
            (1, 4),
            (2, 4),
            (3, 4),
            (0, 4),
            // positive diagonal squares
            (0, 0),
            (1, 1),
            (2, 2),
            (3, 3),
            // negative diagonal squares
            (3, 5),
            (5, 3),
            (6, 2),
            (7, 1)    
        ];

        // Act
        var result = queen.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion
}