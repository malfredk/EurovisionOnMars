using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Infrastructure.DataAccess;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.Countries.CreateCountry;

public interface ICreateCountryRepository
{
    Task<Country> CreateCountry(Country country);
}

public class CreateCountryRepository : ICreateCountryRepository
{
    private readonly DataContext _context;
    private readonly ILogger<CreateCountryRepository> _logger;

    public CreateCountryRepository(ILogger<CreateCountryRepository> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Country> CreateCountry(Country country)
    {
        _logger.LogDebug("Creating country: {country}.", JsonSerializer.Serialize(country));
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
        return country;
    }
}