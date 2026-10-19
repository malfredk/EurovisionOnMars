using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Dto.Countries;

namespace EurovisionOnMars.Api.Features.Countries.CreateCountry;

public interface ICreateCountryService
{
    Task<CountryDto> CreateCountry(NewCountryRequestDto country);
}

public class CreateCountryService : ICreateCountryService
{
    private readonly ILogger<CreateCountryService> _logger;
    private readonly ICreateCountryRepository _repository;
    private readonly ICountryMapper _mapper;

    public CreateCountryService(
        ILogger<CreateCountryService> logger,
        ICreateCountryRepository repository,
        ICountryMapper mapper
        )
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
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
}