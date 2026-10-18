using EurovisionOnMars.Domain.Countries;

namespace EurovisionOnMars.Domain.Test.Countries;

public class CountryNameTest
{
    [Theory]
    [InlineData("østerrike")]
    [InlineData("san marino")]
    [InlineData("bosnia-hercegovina")]
    public void Constructor_ValidValue_CreatesCountryName(string name)
    {
        // acts
        var countryName = new CountryName(name);

        // assert
        Assert.Equal(name, countryName.Value);
    }

    [Theory]
    [InlineData("england")]
    [InlineData("danmark2")]
    [InlineData("danmark_")]
    [InlineData("")]
    public void Constructor_InvalidValue_ThrowsArgumentException(string name)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new CountryName(name));
    }
}
