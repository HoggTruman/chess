using FluentAssertions;
using GameLogic;
using GameLogic.Enums;
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
        var bishop = new BishopPiece(board, 4, 4, PieceColor.White);
        var blockingPiece = new BishopPiece(board, 5, 5, PieceColor.White);
        board.AddPiece(bishop);
        board.AddPiece(blockingPiece);

        List<Square> expected = [
            new(0, 0),
            new(1, 1),
            new(2, 2),
            new(3, 3),
            new(5, 5),
            new(3, 5),
            new(2, 6),
            new(1, 7),
            new(5, 3),
            new(6, 2),
            new(7, 1)            
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

        var bishop = new BishopPiece(board, 4, 4, PieceColor.White);
        var blockingPiece = new BishopPiece(board, 5, 5, PieceColor.White);
        var nonBlockingPiece = new BishopPiece(board, 0, 0, PieceColor.Black);
        board.AddPiece(bishop);
        board.AddPiece(blockingPiece);
        board.AddPiece(nonBlockingPiece);

        List<Square> expected = [
            new(0, 0),
            new(1, 1),
            new(2, 2),
            new(3, 3),
            new(3, 5),
            new(2, 6),
            new(1, 7),
            new(5, 3),
            new(6, 2),
            new(7, 1)            
        ];

        // Act
        var result = bishop.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion
}