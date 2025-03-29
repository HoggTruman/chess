using FluentAssertions;
using BetterGameLogic.Enums;
using BetterGameLogic.Helpers;

namespace BetterGameLogicTests.Helpers;

public class ColorHelpersTests
{
    [Theory]
    [InlineData(PieceColor.White, PieceColor.Black)]
    [InlineData(PieceColor.Black, PieceColor.White)]
    [InlineData(PieceColor.None, PieceColor.None)]
    public void Opposite_ReturnsOppositeColor(PieceColor input, PieceColor expected)
    {
        var result = ColorHelpers.Opposite(input);
        result.Should().Be(expected);
    }
}

