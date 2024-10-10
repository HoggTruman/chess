using GameLogic.Enums;
using GameLogic.Moves;

namespace GameLogicTests.Moves;

public class PromotionMoveTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, PieceType.Queen)]
    [InlineData(0, 1, 2, 3, PieceType.Rook)]
    [InlineData(2, 6, 2, 1, PieceType.Bishop)]
    public void IsEquivalentTo_WhenEquivalent_ReturnsTrue(
        int fromRow, int fromCol, int toRow, int toCol, PieceType promotedTo)
    {
        // Arrange 
        PromotionMove move1 = new(
            (fromRow, fromCol),
            (toRow, toCol),
            promotedTo);

        PromotionMove move2 = new(
            (fromRow, fromCol),
            (toRow, toCol),
            promotedTo);

        // Act
        bool move1Result = move1.IsEquivalentTo(move2);
        bool move2Result = move2.IsEquivalentTo(move1);

        // Assert
        Assert.True(move1Result);
        Assert.True(move2Result);
    }


    [Theory]
    [InlineData(0, 0, 0, 0, PieceType.Queen, 1, 1, 1, 1, PieceType.Bishop)]
    [InlineData(4, 5, 4, 5, PieceType.Queen, 5, 4, 5, 4, PieceType.Queen)]
    [InlineData(0, 1, 2, 3, PieceType.Rook, 5, 6, 7, 6, PieceType.Knight)]
    public void IsEquivalentTo_WhenNotEquivalent_ReturnsFalse(
        int fromRow1, int fromCol1, int toRow1, int toCol1, PieceType promotedTo1,
        int fromRow2, int fromCol2, int toRow2, int toCol2, PieceType promotedTo2)
    {
        // Arrange 
        PromotionMove move1 = new(
            (fromRow1, fromCol1),
            (toRow1, toCol1),
            promotedTo1);

        PromotionMove move2 = new(
            (fromRow2, fromCol2),
            (toRow2, toCol2),
            promotedTo2);

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
        PromotionMove promotionMove = new(
            (0, 0),
            (0, 0),
            PieceType.Queen);

        CastleMove castleMove = new(
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0));

        // Act
        bool result = promotionMove.IsEquivalentTo(castleMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_EnPassantMove_ReturnsFalse()
    {
        // Arrange 
        PromotionMove promotionMove = new(
            (0, 0),
            (0, 0),
            PieceType.Queen);

        EnPassantMove enPassantMove = new(
            (0, 0),
            (0, 0),
            (0, 0));

        // Act
        bool result = promotionMove.IsEquivalentTo(enPassantMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_StandardMove_ReturnsFalse()
    {
        // Arrange 
        PromotionMove promotionMove = new(
            (0, 0),
            (0, 0),
            PieceType.Queen);

        StandardMove standardMove = new(
            (0, 0),
            (0, 0));

        // Act
        bool result = promotionMove.IsEquivalentTo(standardMove);

        // Assert
        Assert.False(result);
    }
}
