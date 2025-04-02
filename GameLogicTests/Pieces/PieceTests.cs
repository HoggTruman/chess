using FluentAssertions;
using GameLogic;
using GameLogic.Constants;
using GameLogic.Enums;
using GameLogic.Moves;
using GameLogic.Pieces;

namespace GameLogicTests.Pieces;

/// <summary>
/// The Piece class is abstract so its methods are tested on child classes
/// </summary>
public class PieceTests
{
    #region HasMoved Tests    

    [Fact]
    public void HasMoved_EmptyMoveHistory_ReturnsFalse()
    {
        // Arrange
        Board board = new();

        var piece = new QueenPiece(board, 4, 4, PieceColor.White);
        board.AddPiece(piece);

        // Act
        var result = piece.HasMoved();

        // Assert
        result.Should().BeFalse();
    }


    [Fact]
    public void HasMoved_StandardMoveInMoveHistory_ReturnsTrue()
    {
        // Arrange
        Board board = new();

        var piece = new QueenPiece(board, 4, 4, PieceColor.White);
        board.AddPiece(piece);

        StandardMove move = new(piece.Square, new(5, 5));
        move.Apply(board);

        // Act
        var result = piece.HasMoved();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void HasMoved_CastleMoveInMoveHistory_ReturnsTrueForRookAndKing()
    {
        // Arrange
        Board board = new();

        var king = new KingPiece(board, StartSquares.WhiteKing, PieceColor.White);
        var rook = new RookPiece(board, StartSquares.WhiteRookK, PieceColor.White);
        board.AddPiece(king);
        board.AddPiece(rook);

        CastleMove move = new(king.Square, new(0, 6), rook.Square, new(0, 5));
        move.Apply(board);

        // Act
        var kingResult = king.HasMoved();
        var rookResult = rook.HasMoved();

        // Assert
        kingResult.Should().BeTrue();
        rookResult.Should().BeTrue();
    }


    [Fact]
    public void HasMoved_EnPassantInMoveHistory_ReturnsTrue()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 4, 4, PieceColor.White);
        var enemyPawn = new PawnPiece(board, 6, 3, PieceColor.Black);
        board.AddPiece(pawn);
        board.AddPiece(enemyPawn);

        StandardMove enemyMove = new(enemyPawn.Square, new(4, 3));
        EnPassantMove enPassantMove = new(pawn.Square, new(5, 3));

        enemyMove.Apply(board);
        enPassantMove.Apply(board);

        // Act
        var result = pawn.HasMoved();

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void HasMoved_PromotionInMoveHistory_ReturnsFalseOnNewPiece()
    {
        // Arrange
        Board board = new();

        var pawn = new PawnPiece(board, 6, 4, PieceColor.White);
        board.AddPiece(pawn);
        Square to = new(7, 4);

        PromotionMove promotionMove = new(pawn.Square, to, PieceType.Queen);
        promotionMove.Apply(board);

        var promotedPiece = board.At(to);

        // Act
        var result = promotedPiece?.HasMoved();

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}