using EurovisionOnMars.Domain.Countries;
using System.Collections.Immutable;

namespace EurovisionOnMars.TestSupport.Countries;

public static class CountryFactory
{
    public static Country CreateInitial(int numberValue = CountryTestData.Number)
    {
        var number = new CountryPosition(numberValue);
        var name = new CountryName(CountryTestData.Name);
        return new Country(number, name);
    }

    public static ImmutableList<Country> CreateInitialSingletonList()
    {
        var countries = new List<Country>();
        countries.Add(CreateInitial());
        return countries.ToImmutableList();
    }

    public static ImmutableList<Country> CreateInitialList(int count)
    {
        var countries = new List<Country>();
        for (int i = 1; i <= count; i++)
        {
            countries.Add(CreateInitial(i));
        }
        return countries.ToImmutableList();
    }
}
