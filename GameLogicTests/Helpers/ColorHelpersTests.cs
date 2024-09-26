using FluentAssertions;
using GameLogic.Enums;
using GameLogic.Helpers;

namespace GameLogicTests.Helpers;

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

