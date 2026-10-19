using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Countries.GetCountries;

public interface IGetCountriesRepository
{
    Task<IReadOnlyCollection<Country>> GetCountries();
}

public class GetCountriesRepository : IGetCountriesRepository
{
    private readonly DataContext _context;
    private readonly ILogger<GetCountriesRepository> _logger;

    public GetCountriesRepository(ILogger<GetCountriesRepository> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IReadOnlyCollection<Country>> GetCountries()
    {
        _logger.LogDebug("Getting all countries.");
        return await _context.Countries.ToListAsync();
    }
}