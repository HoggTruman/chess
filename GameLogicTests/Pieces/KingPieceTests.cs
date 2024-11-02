using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Interfaces;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

public class KingPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(2, 6, PieceColor.White);

        var blockingPiece = board.AddNewPiece<BishopPiece>(2, 7, PieceColor.White);

        List<(int row, int col)> expected = [
            (3, 6),
            (3, 7),
            (2, 7),
            (1, 7),
            (1, 6),
            (1, 5),
            (2, 5),
            (3, 5)
        ];

        // Act
        var result = king.GetTargetedSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion GetTargetedSquares Tests



    #region GetReachableSquares Tests

    [Fact]
    public void GetReachableSquares_ReturnsReachableSquares()
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(2, 6, PieceColor.White);

        var blockingPiece = board.AddNewPiece<BishopPiece>(2, 7, PieceColor.White);
        var nonblockingPiece = board.AddNewPiece<BishopPiece>(1, 7, PieceColor.Black);

        List<(int row, int col)> expected = [
            (3, 6),
            (3, 7),
            (1, 7),
            (1, 6),
            (1, 5),
            (2, 5),
            (3, 5)
        ];

        // Act
        var result = king.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void GetReachableSquares_AtEdge_IncludesOnlyInBounds()
    {
        // Arrange
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(0, 0, PieceColor.White);

        List<(int row, int col)> expected = [
            (1, 0),
            (0, 1),
            (1, 1)
        ];

        // Act
        var result = king.GetReachableSquares();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    #endregion


    #region GetCastleSquares

    [Fact]
    public void GetCastleSquares_WhiteKingSide_ReturnsSquares()
    {
        // Arrange 
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK, PieceColor.White);

        (int row, int col) expectedKingSquare = (7, 6);
        (int row, int col) expectedRookSquare = (7, 5);

        // Act
        var result = king.GetCastleSquares(rook);

        // Assert
        Assert.NotNull(result);
        result.Value.kingTo.Should().Be(expectedKingSquare);
        result.Value.rookTo.Should().Be(expectedRookSquare);
    }


    [Fact]
    public void GetCastleSquares_WhiteQueenSide_ReturnsSquares()
    {
        // Arrange 
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookQ, PieceColor.White);

        (int row, int col) expectedKingSquare = (7, 2);
        (int row, int col) expectedRookSquare = (7, 3);

        // Act
        var result = king.GetCastleSquares(rook);

        // Assert
        Assert.NotNull(result);
        result.Value.kingTo.Should().Be(expectedKingSquare);
        result.Value.rookTo.Should().Be(expectedRookSquare);
    }


    [Fact]
    public void GetCastleSquares_BlackKingSide_ReturnsSquares()
    {
        // Arrange 
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.BlackKing, PieceColor.Black);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.BlackRookK, PieceColor.Black);

        (int row, int col) expectedKingSquare = (0, 6);
        (int row, int col) expectedRookSquare = (0, 5);

        // Act
        var result = king.GetCastleSquares(rook);

        // Assert
        Assert.NotNull(result);
        result.Value.kingTo.Should().Be(expectedKingSquare);
        result.Value.rookTo.Should().Be(expectedRookSquare);
    }


    [Fact]
    public void GetCastleSquares_BlackQueenSide_ReturnsSquares()
    {
        // Arrange 
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.BlackKing, PieceColor.Black);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.BlackRookQ, PieceColor.Black);

        (int row, int col) expectedKingSquare = (0, 2);
        (int row, int col) expectedRookSquare = (0, 3);

        // Act
        var result = king.GetCastleSquares(rook);

        // Assert
        Assert.NotNull(result);
        result.Value.kingTo.Should().Be(expectedKingSquare);
        result.Value.rookTo.Should().Be(expectedRookSquare);
    }


    [Fact]
    public void GetCastleSquares_WhenCantCastle_ReturnsNull()
    {
        // Arrange 
        Board board = new();

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.BlackRookK, PieceColor.White);

        // Act
        var result = king.GetCastleSquares(rook);

        // Assert
        Assert.Null(result);
    }
    
    #endregion



    #region IsChecked Tests

    [Fact]
    public void IsChecked_WithBlackUnderCheck_ReturnsTrue()
    {
        // Arrange
        Board board = new();

        var whiteQueen = board.AddNewPiece<QueenPiece>(0, 0, PieceColor.White);
        var blackKing = board.AddNewPiece<KingPiece>(7, 0, PieceColor.Black);

        // Act
        var result = blackKing.IsChecked();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsChecked_WithWhiteUnderCheck_ReturnsTrue()
    {
        Board board = new();

        var blackQueen = board.AddNewPiece<QueenPiece>(0, 0, PieceColor.Black);
        var whiteKing = board.AddNewPiece<KingPiece>(7, 0, PieceColor.White);

        // Act
        var result = whiteKing.IsChecked();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsChecked_WithNoCheck_ReturnsFalse()
    {
        // Arrange 
        Board board = new();

        var whiteKing = board.AddNewPiece<KingPiece>(0, 0, PieceColor.White);
        var whiteQueen = board.AddNewPiece<QueenPiece>(1, 0, PieceColor.White);
        var blackKing = board.AddNewPiece<KingPiece>(0, 7, PieceColor.Black);
        var blackQueen = board.AddNewPiece<QueenPiece>(1, 7, PieceColor.Black);       

        // Act
        var whiteChecked = whiteKing.IsChecked();
        var blackChecked = blackKing.IsChecked();

        // Assert
        whiteChecked.Should().BeFalse();
        blackChecked.Should().BeFalse();
    }

    #endregion
}