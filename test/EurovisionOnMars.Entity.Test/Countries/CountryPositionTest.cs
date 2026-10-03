using EurovisionOnMars.Entity.Countries;

namespace EurovisionOnMars.Entity.Test.Countries;

public class CountryPositionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(25)]
    [InlineData(26)]
    public void CountryPosition_WithValidPosition_ReturnsCountryPosition(int value)
    {
        // act
        var position = new CountryPosition(value);

        // assert
        Assert.Equal(value, position.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(27)]
    public void CountryPosition_WithInvalidPosition_ThrowsArgumentException(int value)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() =>
            new CountryPosition(value));
    }
}
