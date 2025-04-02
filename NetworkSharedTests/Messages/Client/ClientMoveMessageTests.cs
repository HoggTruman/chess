using FluentAssertions;
using GameLogic.Enums;
using GameLogic.Moves;
using NetworkShared.Messages.Client;

namespace NetworkSharedTests.Messages.Client;

public class ClientMoveMessageTests
{
    ///
    /// Note: these methods don't validate the legality of the moves at all so random values are used
    ///

    [Theory]
    [InlineData(0, 4, 5, 7, 6, 2, 3, 1)]
    [InlineData(0, 1, 2, 3, 4, 5, 6, 7)]
    [InlineData(0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(2, 6, 7, 2, 1, 0, 4, 2)]
    public void Encode_Decode_WithCastleMove_ReturnsOriginal(
        int fromRow, int fromCol, int toRow, int toCol,
        int rookFromRow, int rookFromCol, int rookToRow, int rookToCol)
    {
        // Arrange
        CastleMove move = new(
            new(fromRow, fromCol),
            new(toRow, toCol),
            new(rookFromRow, rookFromCol),
            new(rookToRow, rookToCol));

        // Act
        var encoded = ClientMoveMessage.Encode(move);
        var result = ClientMoveMessage.Decode(encoded);

        // Assert
        result.Should().BeEquivalentTo(move);
    }


    [Theory]
    [InlineData(0, 4, 5, 3)]
    [InlineData(7, 3, 4, 5)]
    [InlineData(0, 0, 3, 3)]
    [InlineData(4, 1, 1, 1)]
    public void Encode_Decode_WithEnPassantMove_ReturnsOriginal(
        int fromRow, int fromCol, int toRow, int toCol)
    {
        // Arrange
        EnPassantMove move = new(
            new(fromRow, fromCol),
            new(toRow, toCol)
        );

        // Act
        var encoded = ClientMoveMessage.Encode(move);
        var result = ClientMoveMessage.Decode(encoded);

        // Assert
        result.Should().BeEquivalentTo(move);
    }


    [Theory]
    [InlineData(1, 2, 3, 4, PieceType.Queen)]
    [InlineData(0, 0, 7, 7, PieceType.Rook)]
    [InlineData(7, 7, 7, 7, PieceType.Knight)]
    [InlineData(4, 5, 2, 0, PieceType.Bishop)]
    public void Encode_Decode_WithPromotionMove_ReturnsOriginal(
        int fromRow, int fromCol, int toRow, int toCol,
        PieceType promotedTo)
    {
        // Arrange
        PromotionMove move = new(
            new(fromRow, fromCol),
            new(toRow, toCol),
            promotedTo
        );

        // Act
        var encoded = ClientMoveMessage.Encode(move);
        var result = ClientMoveMessage.Decode(encoded);

        // Assert
        result.Should().BeEquivalentTo(move);
    }


    [Theory]
    [InlineData(0, 0, 1, 1)]
    [InlineData(2, 3, 5, 7)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(7, 3, 3, 2)]
    public void Encode_Decode_WithStandardMove_ReturnsOriginal(
        int fromRow, int fromCol, int toRow, int toCol)
    {
        // Arrange
        StandardMove move = new(
            new(fromRow, fromCol),
            new(toRow, toCol)
        );

        // Act
        var encoded = ClientMoveMessage.Encode(move);
        var result = ClientMoveMessage.Decode(encoded);

        // Assert
        result.Should().BeEquivalentTo(move);
    }
}
