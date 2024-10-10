using GameLogic.Enums;
using GameLogic.Moves;

namespace GameLogicTests.Moves;

public class EnPassantMoveTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(0, 1, 2, 3, 4, 5)]
    [InlineData(2, 6, 2, 1, 0, 4)]
    public void IsEquivalentTo_WhenEquivalent_ReturnsTrue(
        int fromRow, int fromCol, int toRow, int toCol, int capturedRow, int capturedCol)
    {
        // Arrange 
        EnPassantMove move1 = new(
            (fromRow, fromCol),
            (toRow, toCol),
            (capturedRow, capturedCol));

        EnPassantMove move2 = new(
            (fromRow, fromCol),
            (toRow, toCol),
            (capturedRow, capturedCol));

        // Act
        bool move1Result = move1.IsEquivalentTo(move2);
        bool move2Result = move2.IsEquivalentTo(move1);

        // Assert
        Assert.True(move1Result);
        Assert.True(move2Result);
    }


    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1)]
    [InlineData(4, 5, 4, 5, 4, 5, 5, 4, 5, 4, 5, 4)]
    [InlineData(0, 1, 2, 3, 4, 5, 6, 7, 6, 5, 4, 3)]
    public void IsEquivalentTo_WhenNotEquivalent_ReturnsFalse(
        int fromRow1, int fromCol1, int toRow1, int toCol1, int capturedRow1, int capturedCol1,
        int fromRow2, int fromCol2, int toRow2, int toCol2, int capturedRow2, int capturedCol2)
    {
        // Arrange 
        EnPassantMove move1 = new(
            (fromRow1, fromCol1),
            (toRow1, toCol1),
            (capturedRow1, capturedCol1));

        EnPassantMove move2 = new(
            (fromRow2, fromCol2),
            (toRow2, toCol2),
            (capturedRow2, capturedCol2));

        // Act
        bool move1Result = move1.IsEquivalentTo(move2);
        bool move2Result = move2.IsEquivalentTo(move1);

        // Assert
        Assert.False(move1Result);
        Assert.False(move2Result);
    }


    [Fact]
    public void IsEquivalentTo_CastleMove_ReturnsFalse()
    {
        // Arrange 
        EnPassantMove enPassantMove = new(
            (0, 0),
            (0, 0),
            (0, 0));

        CastleMove castleMove = new(
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0));

        // Act
        bool result = enPassantMove.IsEquivalentTo(castleMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_PromotionMove_ReturnsFalse()
    {
        // Arrange 
        EnPassantMove enPassantMove = new(
            (0, 0),
            (0, 0),
            (0, 0));

        PromotionMove promotionMove = new(
            (0, 0),
            (0, 0),
            PieceType.Queen);

        // Act
        bool result = enPassantMove.IsEquivalentTo(promotionMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_StandardMove_ReturnsFalse()
    {
        // Arrange 
        EnPassantMove enPassantMove = new(
            (0, 0),
            (0, 0),
            (0, 0));

        StandardMove standardMove = new(
            (0, 0),
            (0, 0));

        // Act
        bool result = enPassantMove.IsEquivalentTo(standardMove);

        // Assert
        Assert.False(result);
    }
}
