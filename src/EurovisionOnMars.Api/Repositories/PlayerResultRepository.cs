using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Repositories;

public interface IPlayerResultRepository
{
    Task<ImmutableList<PlayerGameResult>> GetPlayerResults();
    Task<PlayerGameResult?> GetPlayerResult(int playerId);
    Task<PlayerGameResult> UpdatePlayerResult(PlayerGameResult playerResult);
}

public class PlayerResultRepository : IPlayerResultRepository
{
    private readonly DataContext _context;
    private readonly ILogger<PlayerResultRepository> _logger;

    public PlayerResultRepository(DataContext context, ILogger<PlayerResultRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImmutableList<PlayerGameResult>> GetPlayerResults()
    {
        _logger.LogDebug("Getting all player results.");
        var playerResults = await _context.PlayerResults.ToListAsync();
        return playerResults.ToImmutableList();
    }

    public async Task<PlayerGameResult?> GetPlayerResult(int playerId)
    {
        _logger.LogDebug("Getting player result for player with id={playerId}", playerId);
        return await _context.Players
            .Where(p => p.Id == playerId)
            .Select(p => p.PlayerGameResult)
            .FirstOrDefaultAsync();
    }

    public async Task<PlayerGameResult> UpdatePlayerResult(PlayerGameResult playerResult)
    {
        _logger.LogDebug("Updating player result: {playerResult}.", JsonSerializer.Serialize(playerResult));
        var updatedPlayerResult = _context.PlayerResults.Update(playerResult);
        await _context.SaveChangesAsync();
        return updatedPlayerResult.Entity;
    }
}