using EurovisionOnMars.Dto.Countries;

namespace EurovisionOnMars.Api.Features.Countries.GetCountries;

public interface IGetCountriesService
{
    Task<IReadOnlyList<CountryDto>> GetCountries();
}

public class GetCountriesService : IGetCountriesService
{
    private readonly ILogger<GetCountriesService> _logger;
    private readonly IGetCountriesRepository _repository;
    private readonly ICountryMapper _mapper;

    public GetCountriesService(
        ILogger<GetCountriesService> logger,
        IGetCountriesRepository repository,
        ICountryMapper mapper
        )
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CountryDto>> GetCountries()
    {
        var countries = await _repository.GetCountries();
        return countries
            .OrderBy(c => c.Number.Value)
            .Select(c => _mapper.ToDto(c))
            .ToList();
    }
}