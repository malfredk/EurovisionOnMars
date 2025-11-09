using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.Countries;

public interface ICountryService
{
    Task<ImmutableList<Country>> GetCountries();
    Task<Country> CreateCountry(NewCountryRequestDto country);
    Task<Country> UpdateCountry(int id, int rank);
}

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<CountryService> _logger;

    public CountryService(
        ICountryRepository countryRepository,
        ILogger<CountryService> logger
        )
    {
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<ImmutableList<Country>> GetCountries()
    {
        var countries = await _countryRepository.GetCountries();
        return countries.OrderBy(c => c.Number).ToImmutableList();
    }

    public async Task<Country> CreateCountry(NewCountryRequestDto countryDto)
    {
        var country = new Country(countryDto.Number, countryDto.Name);
        return await _countryRepository.CreateCountry(country);
    }

    public async Task<Country> UpdateCountry(int id, int rank)
    {
        var country = await GetCountry(id);
        country.SetActualRank(rank);
        return await _countryRepository.UpdateCountry(country);
    }

    private async Task<Country> GetCountry(int id)
    {
        var country = await _countryRepository.GetCountry(id);
        if (country == null)
        {
            throw new KeyNotFoundException($"No country with id={id} exists");
        }
        return country;
    }
}