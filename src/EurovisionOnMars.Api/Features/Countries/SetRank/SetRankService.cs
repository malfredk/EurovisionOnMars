using EurovisionOnMars.Domain.Countries;

namespace EurovisionOnMars.Api.Features.Countries.SetRank;

public interface ISetRankService
{
    Task SetCountryRank(int id, int rank);
}

public class SetRankService : ISetRankService
{
    private readonly ILogger<SetRankService> _logger;
    private readonly ISetRankRepository _repository;

    public SetRankService(
        ILogger<SetRankService> logger,
        ISetRankRepository repository
        )
    {
        _logger = logger;
        _repository = repository;
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