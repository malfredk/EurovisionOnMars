using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Entity.Tests.Players;

public class PlayerRankTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(100)]
    public void Constructor_ValidRank_CreatesPlayerRank(int value)
    {
        // act
        var rank = new PlayerRank(value);

        // assert
        Assert.Equal(value, rank.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_InvalidRank_ThrowsArgumentException(int value)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new PlayerRank(value));
    }
}