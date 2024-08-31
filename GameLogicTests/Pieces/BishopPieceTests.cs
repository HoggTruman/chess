using FluentAssertions;
using GameLogic;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class BishopPieceTests
{
    #region GetTargetedSquaresTests
    
    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();
        var bishop = board.AddNewPiece<BishopPiece>(4, 4, Color.White);

        var blockingPiece = board.AddNewPiece<BishopPiece>(5, 5, Color.White);

        List<(int row, int col)> expected = [
            (0, 0),
            (1, 1),
            (2, 2),
            (3, 3),
            (5, 5),
            (3, 5),
            (2, 6),
            (1, 7),
            (5, 3),
            (6, 2),
            (7, 1)            
        ];

        // Act
        var result = bishop.GetTargetedSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion



    #region GetReachableSquaresTests

    [Fact]
    public void GetReachableSquares_ReturnsReachableSquares()
    {
        // Arrange
        Board board = new();
        var bishop = board.AddNewPiece<BishopPiece>(4, 4, Color.White);

        var blockingPiece = board.AddNewPiece<BishopPiece>(5, 5, Color.White);
        var nonblockingPiece = board.AddNewPiece<BishopPiece>(0, 0, Color.Black);

        List<(int row, int col)> expected = [
            (0, 0),
            (1, 1),
            (2, 2),
            (3, 3),
            (3, 5),
            (2, 6),
            (1, 7),
            (5, 3),
            (6, 2),
            (7, 1)            
        ];

        // Act
        var result = bishop.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion
}