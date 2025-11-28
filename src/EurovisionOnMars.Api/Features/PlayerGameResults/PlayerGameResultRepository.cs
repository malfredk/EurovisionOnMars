using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

public interface IPlayerGameResultRepository
{
    Task<IReadOnlyList<PlayerGameResult>> GetPlayerGameResults();
    Task<PlayerGameResult?> GetPlayerGameResult(int playerId);
    Task<PlayerGameResult> UpdatePlayerGameResult(PlayerGameResult playerResult);
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

    public async Task<PlayerGameResult?> GetPlayerGameResult(int playerId)
    {
        _logger.LogDebug("Getting player result for player with id={playerId}", playerId);
        return await _context.Players
            .Where(p => p.Id == playerId)
            .Select(p => p.PlayerGameResult)
            .FirstOrDefaultAsync();
    }

    public async Task<PlayerGameResult> UpdatePlayerGameResult(PlayerGameResult playerResult)
    {
        _logger.LogDebug("Updating player result: {playerResult}.", JsonSerializer.Serialize(playerResult));
        var updatedPlayerResult = _context.PlayerGameResults.Update(playerResult);
        await _context.SaveChangesAsync();
        return updatedPlayerResult.Entity;
    }
}