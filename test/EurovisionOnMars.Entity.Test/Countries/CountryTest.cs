using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Test.TestData.Countries;

namespace EurovisionOnMars.Entity.Test.Countries;

public class CountryTest
{
    [Fact]
    public void Constructor_ValidValues_CreatesCountry()
    {
        // arrange
        var number = new CountryPosition(CountryTestData.Number);
        var name = new CountryName(CountryTestData.Name);

        // act
        var country = new Country(number, name);

        // assert
        Assert.Equal(number, country.Number);
        Assert.Equal(name, country.Name);
        Assert.Null(country.ActualRank);
    }

    [Fact]
    public void SetActualRank_Valid()
    {
        // arrange
        var country = CountryFactory.CreateInitialCountry();
        var rank = new CountryPosition(CountryTestData.Rank);

        // act
        country.SetActualRank(rank);

        // assert
        Assert.Equal(rank, country.ActualRank);
    }
}
