using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Repositories;

public interface IPlayerResultRepository
{
    Task<ImmutableList<PlayerResult>> GetPlayerResults();
    Task<PlayerResult?> GetPlayerResult(int playerId);
    Task<PlayerResult> UpdatePlayerResult(PlayerResult playerResult);
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

    public async Task<ImmutableList<PlayerResult>> GetPlayerResults()
    {
        _logger.LogDebug($"Getting all player results");
        var playerResults = await _context.PlayerResults.ToListAsync();
        return playerResults.ToImmutableList();
    }

    public async Task<PlayerResult?> GetPlayerResult(int playerId)
    {
        _logger.LogDebug($"Getting player result for player with id={playerId}");
        return await _context.Players
            .Where(p => p.Id == playerId)
            .Select(p => p.PlayerResult)
            .FirstOrDefaultAsync();
    }

    public async Task<PlayerResult> UpdatePlayerResult(PlayerResult playerResult)
    {
        _logger.LogDebug($"Updating player result with id={playerResult.Id}");
        var updatedPlayerResult = _context.Update(playerResult);
        await _context.SaveChangesAsync();
        return updatedPlayerResult.Entity;
    }
}