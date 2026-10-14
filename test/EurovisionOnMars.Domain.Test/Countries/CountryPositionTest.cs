using EurovisionOnMars.Domain.Countries;

namespace EurovisionOnMars.Domain.Test.Countries;

public class CountryPositionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(25)]
    [InlineData(26)]
    public void Constructor_ValidValue_CreatesCountryPosition(int value)
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
    public void Constructor_InvalidValue_ThrowsArgumentException(int value)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() =>
            new CountryPosition(value));
    }
}
