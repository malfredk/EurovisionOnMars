using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Tests.Players.PlayerRatings;

public class TieBreakDemotionTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Constructor_ValidValue_CreatesPlayerRank(int value)
    {
        // act
        var rank = new TieBreakDemotion(value);

        // assert
        Assert.Equal(value, rank.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_InvalidValue_ThrowsArgumentException(int value)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new TieBreakDemotion(value));
    }
}