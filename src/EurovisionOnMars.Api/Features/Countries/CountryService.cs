using EurovisionOnMars.Dto.Countries;
using EurovisionOnMars.Domain.Countries;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.Countries;

public interface ICountryService
{
    Task<ImmutableList<CountryDto>> GetCountries();
    Task<CountryDto> CreateCountry(NewCountryRequestDto country);
    Task SetCountryRank(int id, int rank);
}

public class CountryService : ICountryService
{
    private readonly ILogger<CountryService> _logger;
    private readonly ICountryRepository _repository;
    private readonly ICountryMapper _mapper;

    public CountryService(
        ILogger<CountryService> logger,
        ICountryRepository repository,
        ICountryMapper mapper
        )
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ImmutableList<CountryDto>> GetCountries()
    {
        var countries = await _repository.GetCountries();
        return countries
            .OrderBy(c => c.Number.Value)
            .Select(c => _mapper.ToDto(c))
            .ToImmutableList();
    }

    public async Task<CountryDto> CreateCountry(NewCountryRequestDto countryDto)
    {
        var country = CreateCountryEntity(countryDto);
        country = await _repository.CreateCountry(country);
        return _mapper.ToDto(country);
    }

    private Country CreateCountryEntity(NewCountryRequestDto countryDto)
    {
        CountryPosition number = new CountryPosition(countryDto.Number);
        CountryName name = new CountryName(countryDto.Name);
        return new Country(number, name);
    }

    public async Task SetCountryRank(int id, int rank)
    {
        var country = await GetCountry(id);
        country.SetActualRank(new CountryPosition(rank));
        await _repository.UpdateCountry(country);
    }

    private async Task<Country> GetCountry(int id)
    {
        var country = await _repository.GetCountry(id);
        if (country == null)
        {
            throw new KeyNotFoundException($"No country with id={id} exists.");
        }
        return country;
    }
}