using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace EurovisionOnMars.Api.Services;

public interface ICountryService
{
    Task<ImmutableList<Country>> GetCountries();
    Task<Country> GetCountry(int id);
    Task<Country> CreateCountry(Country country);
    Task<Country> UpdateCountry(Country country);
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
        return await _countryRepository.GetCountries();
    }

    public async Task<Country> GetCountry(int id)
    {
        var country = await _countryRepository.GetCountry(id);
        if (country == null)
        {
            throw new KeyNotFoundException($"No country with id={id} exists");
        }
        return country;
    }

    public async Task<Country> CreateCountry(Country country)
    {
        SanitizeName(country.Name);
        return await _countryRepository.CreateCountry(country);
    }

    public async Task<Country> UpdateCountry(Country country)
    {
        return await _countryRepository.UpdateCountry(country);
    }

    private void SanitizeName(string name)
    {
        string pattern = @"^[a-zA-ZæøåÆØÅ]*$";
        var isValid = !string.IsNullOrEmpty(name)
            && Regex.IsMatch(name, pattern);
        
        if (!isValid)
        {
            throw new ArgumentException("Invalid name of country");
        }
    }
}