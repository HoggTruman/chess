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

        var piece = board.AddNewPiece<QueenPiece>(4, 4, PieceColor.White);

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

        var piece = board.AddNewPiece<QueenPiece>(4, 4, PieceColor.White);

        StandardMove move = new(piece.Square, (5, 5));
        board.StandardMove(move);

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

        var king = board.AddNewPiece<KingPiece>(StartSquares.WhiteKing, PieceColor.White);
        var rook = board.AddNewPiece<RookPiece>(StartSquares.WhiteRookK, PieceColor.White);

        CastleMove move = new(king.Square, (0, 6), rook.Square, (0, 5));
        board.Castle(move);

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

        var pawn = board.AddNewPiece<PawnPiece>(4, 4, PieceColor.White);
        var enemyPawn = board.AddNewPiece<PawnPiece>(6, 3, PieceColor.Black);

        StandardMove enemyMove = new(enemyPawn.Square, (4, 3));
        EnPassantMove enPassantMove = new(pawn.Square, (5, 3), (4, 3));

        board.StandardMove(enemyMove);
        board.EnPassant(enPassantMove);

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

        var pawn = board.AddNewPiece<PawnPiece>(6, 4, PieceColor.White);
        (int row, int col) to = (7, 4);

        PromotionMove promotionMove = new(pawn.Square, to, PieceType.Queen);
        board.PawnPromote(promotionMove);

        var promotedPiece = board.State[to.row, to.col];

        // Act
        var result = promotedPiece?.HasMoved();

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}