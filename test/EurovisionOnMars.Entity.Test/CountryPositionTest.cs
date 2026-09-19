namespace EurovisionOnMars.Entity.Test;

public class CountryPositionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(13)]
    [InlineData(25)]
    [InlineData(26)]
    public void Create_WithValidPosition_ReturnsCountryPosition(int value)
    {
        var position = new CountryPosition(value);

        Assert.Equal(value, position.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(27)]
    [InlineData(100)]
    public void Create_WithInvalidPosition_ThrowsArgumentException(int value)
    {
        Assert.Throws<ArgumentException>(() =>
            new CountryPosition(value));
    }
}
