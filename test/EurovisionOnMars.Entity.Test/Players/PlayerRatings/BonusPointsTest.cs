using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Tests.Players.PlayerRatings;

public class BonusPointsTest
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-4)]
    [InlineData(-6)]
    [InlineData(-8)]
    [InlineData(-10)]
    [InlineData(-12)]
    [InlineData(-15)]
    [InlineData(-18)]
    [InlineData(-25)]
    public void Constructor_ValidValue_CreatesBonusPoints(int value)
    {
        // act
        var bonusPoints = new BonusPoints(value);

        // assert
        Assert.Equal(value, bonusPoints.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-3)]
    [InlineData(-5)]
    [InlineData(-20)]
    [InlineData(-26)]
    public void Constructor_InvalidValue_ThrowsArgumentException(int value)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new BonusPoints(value));
    }

    [Theory]
    [InlineData(1, -25)]
    [InlineData(2, -18)]
    [InlineData(3, -15)]
    [InlineData(4, -12)]
    [InlineData(5, -10)]
    [InlineData(6, -8)]
    [InlineData(7, -6)]
    [InlineData(8, -4)]
    [InlineData(9, -2)]
    [InlineData(10, -1)]
    public void FromRank_RankWithBonus_ReturnsCorrectBonus(
        int rank,
        int expectedBonus)
    {
        // arrange
        var countryPosition = new CountryPosition(rank);

        // act
        var bonusPoints = BonusPoints.FromRank(countryPosition);

        // assert
        Assert.Equal(expectedBonus, bonusPoints.Value);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(15)]
    [InlineData(26)]
    public void FromRank_RankWithoutBonus_ReturnsZero(int rank)
    {
        // arrange
        var countryPosition = new CountryPosition(rank);

        // act
        var bonusPoints = BonusPoints.FromRank(countryPosition);

        // assert
        Assert.Equal(0, bonusPoints.Value);
    }
}