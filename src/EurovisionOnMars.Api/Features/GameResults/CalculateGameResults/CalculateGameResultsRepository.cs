using EurovisionOnMars.Domain.DataAccess;
using EurovisionOnMars.Domain.Players;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;

public interface ICalculateGameResultsRepository
{
    Task<IReadOnlyList<Player>> GetPlayers();
    Task SaveChanges();
}

public class CalculateGameResultsRepository : ICalculateGameResultsRepository
{
    private readonly ILogger<ICalculateGameResultsRepository> _logger;
    private readonly DataContext _context;

    public CalculateGameResultsRepository(
        ILogger<ICalculateGameResultsRepository> logger,
        DataContext context
        )
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IReadOnlyList<Player>> GetPlayers()
    {
        _logger.LogDebug("Getting all players for game result calculation.");

        return await _context.Players
            .Include(p => p.PlayerGameResult)
            .Include(p => p.PlayerRatings)
                .ThenInclude(r => r.Country)
            .Include(p => p.PlayerRatings)
                .ThenInclude(r => r.Prediction)
            .Include(p => p.PlayerRatings)
                .ThenInclude(r => r.RatingGameResult)
            .ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}