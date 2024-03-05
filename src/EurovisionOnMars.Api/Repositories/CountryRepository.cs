using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Repositories;

public interface ICountryRepository
{
    Task<ImmutableList<Country>> GetCountries();
    Task<Country?> GetCountry(int id);
    Task<Country> CreateCountry(Country country);
    Task<Country> UpdateCountry(Country country);
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

    public async Task<Country?> GetCountry(int id)
    {
        _logger.LogDebug($"Getting country with id={id}");
        return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Country> CreateCountry(Country country)
    {
        _logger.LogDebug($"Creating country number {country.Number}");
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
        return country;
    }

    public async Task<Country> UpdateCountry(Country country)
    {
        _logger.LogDebug($"Updating country with id={country.Id}");
        var updatedCountry = _context.Countries.Update(country);
        await _context.SaveChangesAsync();
        return updatedCountry.Entity;
    }
}