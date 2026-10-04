using EurovisionOnMars.Entity.Countries;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test.TestData.Countries;

public static class CountryFactory
{
    public static Country CreateInitialCountry(int numberValue = CountryTestData.Number)
    {
        var number = new CountryPosition(numberValue);
        var name = new CountryName(CountryTestData.Name);
        return new Country(number, name);
    }

    public static Country CreateRankedCountry()
    {
        var country = CreateInitialCountry();

        var rank = new CountryPosition(CountryTestData.Rank);
        country.SetActualRank(rank);

        return country;
    }

    public static ImmutableList<Country> CreateInitialSingletonList()
    {
        var countries = new List<Country>();
        countries.Add(CreateInitialCountry());
        return countries.ToImmutableList();
    }

    public static ImmutableList<Country> CreateInitialListWith2Countries()
    {
        var countries = new List<Country>();
        countries.Add(CreateInitialCountry(1));
        countries.Add(CreateInitialCountry(2));
        return countries.ToImmutableList();
    }

    public static ImmutableList<Country> CreateInitialListWith4Countries()
    {
        var countries = new List<Country>();
        countries.Add(CreateInitialCountry(1));
        countries.Add(CreateInitialCountry(2));
        countries.Add(CreateInitialCountry(20));
        countries.Add(CreateInitialCountry(4));
        return countries.ToImmutableList();
    }
}
