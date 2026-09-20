using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.GameResults;

public interface IGameResultRepository
{
    Task<IReadOnlyList<Player>> GetPlayers();
    Task SaveChanges();
}

public class GameResultRepository : IGameResultRepository
{
    private readonly DataContext _context;
    private readonly ILogger<IGameResultRepository> _logger;

    public GameResultRepository(DataContext context, ILogger<IGameResultRepository> logger)
    {
        _context = context;
        _logger = logger;
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