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
    private static int MIN_NUMBER = 1;
    private static int MAX_NUMBER = 26;
    private static ImmutableList<string> POSSIBLE_PARTICIPANTS = new List<string>
    {
        "australia",
        "tsjekkia",
        "armenia",
        "serbia",
        "moldova",
        "ukraina",
        "albania",
        "litauen",
        "polen",
        "kroatia",
        "estland",
        "slovenia",
        "kypros",
        "israel",
        "italia",
        "portugal",
        "østerrike",
        "finland",
        "norge",
        "spania",
        "sverige",
        "sveits",
        "belgia",
        "frankrike",
        "storbritannia",
        "tyskland",
        "aserbajdsjan",
        "romania",
        "island",
        "hellas",
        "nederland",
        "san marino",
        "bulgaria",
        "russland",
        "malta",
        "belarus",
        "nord-makedonia",
        "danmark",
        "ungarn",
        "irland",
        "georgia",
        "latvia",
        "montenegro",
        "bosnia-hercegovina",
        "tyrkia",
        "slovakia",
        "luxembourg",
        "monaco"
    }.ToImmutableList();

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
        ValidateName(country.Name);
        ValidateNumber(country.Number);
        return await _countryRepository.CreateCountry(country);
    }

    public async Task<Country> UpdateCountry(Country country)
    {
        ValidateNumber(country.Ranking);
        return await _countryRepository.UpdateCountry(country);
    }

    private void ValidateNumber(int? number)
    {
        var isValid = number != null
            && number >= MIN_NUMBER
            && number <= MAX_NUMBER;

        if (!isValid)
        {
            throw new ArgumentException("Invalid number or ranking for country");
        }
    }

    private void ValidateName(string name)
    {
        string pattern = @"^[a-zæøå -]*$";
        var isValid = !string.IsNullOrEmpty(name)
            && Regex.IsMatch(name, pattern)
            && POSSIBLE_PARTICIPANTS.Contains(name);
        
        if (!isValid)
        {
            throw new ArgumentException("Invalid name of country");
        }
    }
}