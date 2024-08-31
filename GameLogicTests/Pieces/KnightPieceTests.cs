using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class KnightPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();
        var knight = board.AddNewPiece<KnightPiece>(4, 4, Color.White);


        var blockingPiece1 = board.AddNewPiece<KnightPiece>(4, 3, Color.White);
        var blockingPiece2 = board.AddNewPiece<KnightPiece>(4, 5, Color.White);
        var blockingPiece3 = board.AddNewPiece<KnightPiece>(3, 4, Color.White);
        var blockingPiece4 = board.AddNewPiece<KnightPiece>(5, 4, Color.White);
        var blockingPiece5 = board.AddNewPiece<KnightPiece>(5, 5, Color.White);
        var blockingPiece6 = board.AddNewPiece<KnightPiece>(3, 3, Color.White);
        var blockingPiece7 = board.AddNewPiece<KnightPiece>(5, 3, Color.White);
        var blockingPiece8 = board.AddNewPiece<KnightPiece>(3, 5, Color.White);

        List<(int row, int col)> expected = [
            (5, 6),
            (5, 2),
            (3, 6),
            (3, 2),
            (6, 5),
            (6, 3),
            (2, 5),
            (2, 3),            
        ];

        // Act
        var result = knight.GetTargetedSquares();

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

        var knight = board.AddNewPiece<KnightPiece>(4, 4, Color.White);

        var blockingPiece1 = board.AddNewPiece<KnightPiece>(4, 3, Color.White);
        var blockingPiece2 = board.AddNewPiece<KnightPiece>(4, 5, Color.White);
        var blockingPiece3 = board.AddNewPiece<KnightPiece>(3, 4, Color.White);
        var blockingPiece4 = board.AddNewPiece<KnightPiece>(5, 4, Color.White);
        var blockingPiece5 = board.AddNewPiece<KnightPiece>(5, 5, Color.White);
        var blockingPiece6 = board.AddNewPiece<KnightPiece>(3, 3, Color.White);
        var blockingPiece7 = board.AddNewPiece<KnightPiece>(5, 3, Color.White);
        var blockingPiece8 = board.AddNewPiece<KnightPiece>(3, 5, Color.White);

        var sameColorPieceOnTargetedSquare = board.AddNewPiece<KnightPiece>(5, 6, Color.White);
        var enemyPieceOnTargetedSquare = board.AddNewPiece<KnightPiece>(3, 6, Color.Black);

        List<(int row, int col)> expected = [
            (5, 2),
            (3, 6),
            (3, 2),
            (6, 5),
            (6, 3),
            (2, 5),
            (2, 3),            
        ];

        // Act
        var result = knight.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    #endregion
}