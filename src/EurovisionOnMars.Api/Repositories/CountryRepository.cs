using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Repositories;

public interface ICountryRepository
{
    Task<ImmutableList<Country>> GetCountries();
}

public class CountryRepository : ICountryRepository
{
    private readonly DataContext _context;
    private readonly ILogger<CountryRepository> _logger;

    public CountryRepository(ILogger<CountryRepository> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ImmutableList<Country>> GetCountries()
    {
        _logger.LogDebug("Getting all countries");
        var countries = await _context.Countries.ToListAsync();
        return countries.ToImmutableList();
    }
}