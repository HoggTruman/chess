using GameLogic.Enums;
using GameLogic.Moves;

namespace GameLogicTests.Moves;

public class CastleMoveTests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(0, 1, 2, 3, 4, 5, 6, 7)]
    [InlineData(2, 6, 2, 1, 0, 4, 3, 6)]
    public void IsEquivalentTo_WhenEquivalent_ReturnsTrue(
        int fromRow, int fromCol, int toRow, int toCol, int rookFromRow, int rookFromCol, int rookToRow, int rookToCol)
    {
        // Arrange 
        CastleMove move1 = new(
            (fromRow, fromCol),
            (toRow, toCol),
            (rookFromRow, rookFromCol),
            (rookToRow, rookToCol));

        CastleMove move2 = new(
            (fromRow, fromCol),
            (toRow, toCol),
            (rookFromRow, rookFromCol),
            (rookToRow, rookToCol));

        // Act
        bool move1Result = move1.IsEquivalentTo(move2);
        bool move2Result = move2.IsEquivalentTo(move1);

        // Assert
        Assert.True(move1Result);
        Assert.True(move2Result);
    }


    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1)]
    [InlineData(4, 5, 4, 5, 4, 5, 4, 5, 5, 4, 5, 4, 5, 4, 5, 4)]
    [InlineData(0, 1, 2, 3, 4, 5, 6, 7, 6, 5, 4, 3, 2, 1, 0, 7)]
    public void IsEquivalentTo_WhenNotEquivalent_ReturnsFalse(
        int fromRow1, int fromCol1, int toRow1, int toCol1, int rookFromRow1, int rookFromCol1, int rookToRow1, int rookToCol1,
        int fromRow2, int fromCol2, int toRow2, int toCol2, int rookFromRow2, int rookFromCol2, int rookToRow2, int rookToCol2)
    {
        // Arrange 
        CastleMove move1 = new(
            (fromRow1, fromCol1),
            (toRow1, toCol1),
            (rookFromRow1, rookFromCol1),
            (rookToRow1, rookToCol1));

        CastleMove move2 = new(
            (fromRow2, fromCol2),
            (toRow2, toCol2),
            (rookFromRow2, rookFromCol2),
            (rookToRow2, rookToCol2));

        // Act
        bool move1Result = move1.IsEquivalentTo(move2);
        bool move2Result = move2.IsEquivalentTo(move1);

        // Assert
        Assert.False(move1Result);
        Assert.False(move2Result);
    }


    [Fact]
    public void IsEquivalentTo_EnPassantMove_ReturnsFalse()
    {
        // Arrange 
        CastleMove castleMove = new(
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0));

        EnPassantMove enPassantMove = new(
            (0, 0),
            (0, 0),
            (0, 0));

        // Act
        bool result = castleMove.IsEquivalentTo(enPassantMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_PromotionMove_ReturnsFalse()
    {
        // Arrange 
        CastleMove castleMove = new(
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0));

        PromotionMove promotionMove = new(
            (0, 0),
            (0, 0),
            PieceType.Queen);

        // Act
        bool result = castleMove.IsEquivalentTo(promotionMove);

        // Assert
        Assert.False(result);
    }


    [Fact]
    public void IsEquivalentTo_StandardMove_ReturnsFalse()
    {
        // Arrange 
        CastleMove castleMove = new(
            (0, 0),
            (0, 0),
            (0, 0),
            (0, 0));

        StandardMove standardMove = new(
            (0, 0),
            (0, 0));

        // Act
        bool result = castleMove.IsEquivalentTo(standardMove);

        // Assert
        Assert.False(result);
    }
}
