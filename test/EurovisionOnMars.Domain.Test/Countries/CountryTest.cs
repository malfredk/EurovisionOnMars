using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.TestSupport.Countries;

namespace EurovisionOnMars.Domain.Test.Countries;

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
    public void SetActualRank_ValidValue_SetsActualRank()
    {
        // arrange
        var country = CountryFactory.CreateInitial();
        var rank = new CountryPosition(CountryTestData.Rank);

        // act
        country.SetActualRank(rank);

        // assert
        Assert.Equal(rank, country.ActualRank);
    }
}
