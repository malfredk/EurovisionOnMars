using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Players.RateCountry;

public interface IRateCountryRepository
{
    Task<Player?> GetPlayer(int playerId);
    Task SaveChanges();
}

public class RateCountryRepository : IRateCountryRepository
{
    private readonly ILogger<RateCountryRepository> _logger;
    private readonly DataContext _context;

    public RateCountryRepository(
        ILogger<RateCountryRepository> logger,
        DataContext context
        )
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Player?> GetPlayer(int playerId)
    {
        _logger.LogDebug("Getting player for country rating.");
        return await _context.Players
            .Include(p => p.PlayerRatings)
                .ThenInclude(pr => pr.Prediction)
            .SingleOrDefaultAsync(p => p.Id == playerId);
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}