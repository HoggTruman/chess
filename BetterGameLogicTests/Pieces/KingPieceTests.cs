using FluentAssertions;
using BetterGameLogic;
using BetterGameLogic.Constants;
using BetterGameLogic.Enums;
using BetterGameLogic.Pieces;

namespace BetterGameLogicTests.Pieces;

public class KingPieceTests
{
    #region GetTargetedSquares Tests

    [Fact]
    public void GetTargetedSquares_ReturnsTargetedSquares()
    {
        // Arrange
        Board board = new();

        var king = new KingPiece(board, 2, 6, PieceColor.White);
        var blockingPiece = new BishopPiece(board, 2, 7, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(blockingPiece);

        List<Square> expected = [
            new(3, 6),
            new(3, 7),
            new(2, 7),
            new(1, 7),
            new(1, 6),
            new(1, 5),
            new(2, 5),
            new(3, 5)
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

        var king = new KingPiece(board, 2, 6, PieceColor.White);
        var blockingPiece = new BishopPiece(board, 2, 7, PieceColor.White);
        var nonBlockingPiece = new BishopPiece(board, 1, 7, PieceColor.Black);
        board.AddPiece(king);
        board.AddPiece(blockingPiece);
        board.AddPiece(nonBlockingPiece);

        List<Square> expected = [
            new(3, 6),
            new(3, 7),
            new(1, 7),
            new(1, 6),
            new(1, 5),
            new(2, 5),
            new(3, 5)
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

        var king = new KingPiece(board, 0, 0, PieceColor.White);
        board.AddPiece(king);

        List<Square> expected = [
            new(1, 0),
            new(0, 1),
            new(1, 1)
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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        Square expectedKingSquare = new(7, 6);
        Square expectedRookSquare = new(7, 5);

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookQ, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        Square expectedKingSquare = new(7, 2);
        Square expectedRookSquare = new(7, 3);

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

        var king = new KingPiece(board, StartSquares.BlackKing, PieceColor.Black);
        var rook = new RookPiece(board, StartSquares.BlackRookK, PieceColor.Black);
        board.AddPiece(king);
        board.AddPiece(rook);

        Square expectedKingSquare = new(0, 6);
        Square expectedRookSquare = new(0, 5);

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

        var king = new KingPiece(board, StartSquares.BlackKing, PieceColor.Black);
        var rook = new RookPiece(board, StartSquares.BlackRookQ, PieceColor.Black);
        board.AddPiece(king);
        board.AddPiece(rook);

        Square expectedKingSquare = new(0, 2);
        Square expectedRookSquare = new(0, 3);

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

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.BlackRookK, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        // Act
        var result = king.GetCastleSquares(rook);

        // Assert
        Assert.Null(result);
    }
    
    #endregion



    #region IsUnderCheck Tests

    [Fact]
    public void IsUnderCheck_WithBlackUnderCheck_ReturnsTrue()
    {
        // Arrange
        Board board = new();

        var whiteQueen = new QueenPiece(board, 0, 0, PieceColor.White);
        var blackKing = new KingPiece(board, 7, 0, PieceColor.Black);
        board.AddPiece(whiteQueen);
        board.AddPiece(blackKing);

        // Act
        var result = blackKing.IsUnderCheck();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsUnderCheck_WithWhiteUnderCheck_ReturnsTrue()
    {
        Board board = new();

        var blackQueen = new QueenPiece(board, 0, 0, PieceColor.Black);
        var whiteKing = new KingPiece(board, 7, 0, PieceColor.White);
        board.AddPiece(blackQueen);
        board.AddPiece(whiteKing);

        // Act
        var result = whiteKing.IsUnderCheck();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void IsUnderCheck_WithNoCheck_ReturnsFalse()
    {
        // Arrange 
        Board board = new();

        var whiteKing = new KingPiece(board, 0, 0, PieceColor.White);
        var whiteQueen = new QueenPiece(board, 1, 0, PieceColor.White);
        var blackKing = new KingPiece(board, 0, 7, PieceColor.Black);
        var blackQueen = new QueenPiece(board, 1, 7, PieceColor.Black);
        board.AddPiece(whiteKing);
        board.AddPiece(whiteQueen);
        board.AddPiece(blackKing);
        board.AddPiece(blackQueen);

        // Act
        var whiteChecked = whiteKing.IsUnderCheck();
        var blackChecked = blackKing.IsUnderCheck();

        // Assert
        whiteChecked.Should().BeFalse();
        blackChecked.Should().BeFalse();
    }

    #endregion
}