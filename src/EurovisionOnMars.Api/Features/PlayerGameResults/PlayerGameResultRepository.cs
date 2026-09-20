using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

public interface IPlayerGameResultRepository
{
    Task<IReadOnlyList<PlayerGameResult>> GetPlayerGameResults();
}

public class PlayerGameResultRepository : IPlayerGameResultRepository
{
    private readonly DataContext _context;
    private readonly ILogger<PlayerGameResultRepository> _logger;

    public PlayerGameResultRepository(DataContext context, ILogger<PlayerGameResultRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PlayerGameResult>> GetPlayerGameResults()
    {
        _logger.LogDebug("Getting all player game results.");
        return await _context.PlayerGameResults
            .Include(pgr => pgr.Player)
            .ToListAsync();
    }
}