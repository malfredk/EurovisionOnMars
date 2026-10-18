using EurovisionOnMars.Domain.Countries;

namespace EurovisionOnMars.TestSupport.Countries;

public static class CountryFactory
{
    public static Country CreateInitial(int numberValue = CountryTestData.Number)
    {
        var number = new CountryPosition(numberValue);
        var name = new CountryName(CountryTestData.Name);
        return new Country(number, name);
    }

    public static IReadOnlyCollection<Country> CreateInitialSingletonList()
    {
        var countries = new List<Country>();
        countries.Add(CreateInitial());
        return countries;
    }

    public static IReadOnlyCollection<Country> CreateInitialList(int count)
    {
        var countries = new List<Country>();
        for (int i = 1; i <= count; i++)
        {
            countries.Add(CreateInitial(i));
        }
        return countries;
    }
}
