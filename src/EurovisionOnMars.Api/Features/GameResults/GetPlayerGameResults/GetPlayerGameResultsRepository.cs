using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;

public interface IGetPlayerGameResultsRepository
{
    Task<IReadOnlyList<PlayerGameResult>> GetPlayerGameResults();
}

public class GetPlayerGameResultsRepository : IGetPlayerGameResultsRepository
{
    private readonly ILogger<GetPlayerGameResultsRepository> _logger;
    private readonly DataContext _context;

    public GetPlayerGameResultsRepository(
        ILogger<GetPlayerGameResultsRepository> logger,
        DataContext context
        )
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IReadOnlyList<PlayerGameResult>> GetPlayerGameResults()
    {
        _logger.LogDebug("Getting all player game results with their player.");
        return await _context.PlayerGameResults
            .AsNoTracking()
            .Include(pgr => pgr.Player)
            .ToListAsync();
    }
}