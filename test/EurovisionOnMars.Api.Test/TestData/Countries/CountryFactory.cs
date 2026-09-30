using EurovisionOnMars.Entity.Countries;

namespace EurovisionOnMars.Api.Test.TestData.Countries;

public static class CountryFactory
{
    public static Country CreateInitialCountry(int numberValue = CountryTestData.Number)
    {
        var number = new CountryPosition(numberValue);
        var name = new CountryName(CountryTestData.Name);
        return new Country(number, name)
        {
            Id = CountryTestData.Id
        };
    }

    public static List<Country> CreateInitialCountries()
    {
        var countries = new List<Country>();
        countries.Add(CreateInitialCountry());

        return countries;
    }

    public static Country CreateRankedCountry()
    {
        var country = CreateInitialCountry();

        var rank = new CountryPosition(CountryTestData.Rank);
        country.SetActualRank(rank);

        return country;
    }
}
