using GameLogic.Enums;
using GameLogic.Moves;

namespace GameLogicTests.Moves;

public class StandardMoveTests
{
    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(0, 1, 2, 3)]
    [InlineData(2, 6, 2, 1)]
    public void IsEquivalentTo_WhenEquivalent_ReturnsTrue(
        int fromRow, int fromCol, int toRow, int toCol)
    {
        // Arrange 
        StandardMove move1 = new(
            (fromRow, fromCol),
            (toRow, toCol));

        StandardMove move2 = new(
            (fromRow, fromCol),
            (toRow, toCol));

        // Act
        bool move1Result = move1.IsEquivalentTo(move2);
        bool move2Result = move2.IsEquivalentTo(move1);

        // Assert
        Assert.True(move1Result);
        Assert.True(move2Result);
    }


    [Theory]
    [InlineData(0, 0, 0, 0, 1, 1, 1, 1)]
    [InlineData(4, 5, 4, 5, 5, 4, 5, 4)]
    [InlineData(0, 1, 2, 3, 5, 6, 7, 6)]
    public void IsEquivalentTo_WhenNotEquivalent_ReturnsFalse(
        int fromRow1, int fromCol1, int toRow1, int toCol1,
        int fromRow2, int fromCol2, int toRow2, int toCol2)
    {
        // Arrange 
        StandardMove move1 = new(
            (fromRow1, fromCol1),
            (toRow1, toCol1));

        StandardMove move2 = new(
            (fromRow2, fromCol2),
            (toRow2, toCol2));

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
        StandardMove standardMove = new(
            (0, 0),
            (0, 0));

        CastleMove castleMove = new(
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0));

        // Act
        bool result = standardMove.IsEquivalentTo(castleMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_EnPassantMove_ReturnsFalse()
    {
        // Arrange 
        StandardMove standardMove = new(
            (0, 0),
            (0, 0));

        EnPassantMove enPassantMove = new(
            (0, 0),
            (0, 0),
            (0, 0));

        // Act
        bool result = standardMove.IsEquivalentTo(enPassantMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_PromotionMove_ReturnsFalse()
    {
        // Arrange 
        StandardMove standardMove = new(
            (0, 0),
            (0, 0));

        PromotionMove promotionMove = new(
            (0, 0),
            (0, 0),
            PieceType.Queen);

        // Act
        bool result = standardMove.IsEquivalentTo(promotionMove);

        // Assert
        Assert.False(result);
    }
}
