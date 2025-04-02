using GameLogic;
using GameLogic.Enums;
using GameLogic.Pieces;
using FluentAssertions;

namespace GameLogicTests.Pieces;

public class KnightPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();
        var knight = new KnightPiece(board, 4, 4, PieceColor.White);
        board.AddPiece(knight);

        // blocking pieces
        board.AddPiece(new KnightPiece(board, 4, 3, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 4, 5, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 3, 4, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 5, 4, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 5, 5, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 3, 3, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 5, 3, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 3, 5, PieceColor.White));

        List<Square> expected = [
            new(5, 6),
            new(5, 2),
            new(3, 6),
            new(3, 2),
            new(6, 5),
            new(6, 3),
            new(2, 5),
            new(2, 3),            
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

        var knight = new KnightPiece(board, 4, 4, PieceColor.White);
        board.AddPiece(knight);

        var sameColorPieceOnTargetedSquare = new KnightPiece(board, 5, 6, PieceColor.White);
        board.AddPiece(sameColorPieceOnTargetedSquare);
        var enemyPieceOnTargetedSquare = new KnightPiece(board, 3, 6, PieceColor.Black);
        board.AddPiece(enemyPieceOnTargetedSquare);

        // blocking pieces
        board.AddPiece(new KnightPiece(board, 4, 3, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 4, 5, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 3, 4, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 5, 4, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 5, 5, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 3, 3, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 5, 3, PieceColor.White));
        board.AddPiece(new KnightPiece(board, 3, 5, PieceColor.White));
        

        List<Square> expected = [
            new(5, 2),
            new(3, 6),
            new(3, 2),
            new(6, 5),
            new(6, 3),
            new(2, 5),
            new(2, 3),            
        ];

        // Act
        var result = knight.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_AtEdge_IncludesOnlyInBounds()
    {
        // Arrange
        Board board = new();

        var knight = new KnightPiece(board, 0, 0, PieceColor.White);
        board.AddPiece(knight);

        List<Square> expected = [
            new(1, 2),
            new(2, 1),
        ];

        // Act
        var result = knight.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    #endregion
}