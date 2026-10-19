using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.Countries.SetRank;

public interface ISetRankRepository
{
    Task<Country?> GetCountry(int id);
    Task UpdateCountry(Country country);
}

public class SetRankRepository : ISetRankRepository
{
    private readonly DataContext _context;
    private readonly ILogger<SetRankRepository> _logger;

    public SetRankRepository(ILogger<SetRankRepository> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Country?> GetCountry(int id)
    {
        _logger.LogDebug("Getting country with id={id}.", id);
        return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateCountry(Country country)
    {
        _logger.LogDebug("Updating country: {country}.", JsonSerializer.Serialize(country));
        var updatedCountry = _context.Countries.Update(country);
        await _context.SaveChangesAsync();
    }
}