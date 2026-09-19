using EurovisionOnMars.Entity.Countries;

namespace EurovisionOnMars.Entity.Test.Countries;

public class CountryNameTest
{
    [Theory]
    [InlineData("østerrike")]
    [InlineData("san marino")]
    [InlineData("bosnia-hercegovina")]
    public void CountryName_Valid(string name)
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
    public void CountryName_Invalid(string name)
    {
        // act & assert
        Assert.Throws<ArgumentException>(() => new CountryName(name));
    }
}
