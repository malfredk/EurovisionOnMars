using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Dto.Requests;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace EurovisionOnMars.Api.Services;

public interface ICountryService
{
    Task<ImmutableList<Country>> GetCountries();
    Task<Country> CreateCountry(NewCountryRequestDto country);
    Task<Country> UpdateCountry(int id, int ranking);
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
        var countries = await _countryRepository.GetCountries();
        return countries.OrderBy(c => c.Number).ToImmutableList();
    }

    public async Task<Country> CreateCountry(NewCountryRequestDto countryDto)
    {
        ValidateName(countryDto.Name);
        ValidateNumber(countryDto.Number);
        var country = CreateEntity(countryDto);
        return await _countryRepository.CreateCountry(country);
    }

    public async Task<Country> UpdateCountry(int id, int ranking)
    {
        ValidateNumber(ranking);
        var country = await GetCountry(id);
        var updatedCountry = UpdateEntity(country, ranking);
        return await _countryRepository.UpdateCountry(updatedCountry);
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

    private Country CreateEntity(NewCountryRequestDto dto)
    {
        return new Country
        {
            Name = dto.Name,
            Number = dto.Number
        };
    }

    private Country UpdateEntity(Country entity, int ranking)
    {
        entity.Ranking = ranking;
        return entity;
    }
}