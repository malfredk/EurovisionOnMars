using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.Players;

public interface IPlayerRepository
{
    Task<Player?> GetPlayer(int id);
    Task<Player?> GetPlayer(string username);
    Task<Player> CreatePlayer(Player player);
}

public class PlayerRepository : IPlayerRepository
{
    private readonly DataContext _context;
    private readonly ILogger<PlayerRepository> _logger;

    public PlayerRepository(DataContext context, ILogger<PlayerRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Player?> GetPlayer(int id)
    {
        _logger.LogDebug("Getting player with id={id}.", id);
        return await _context.Players.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Player?> GetPlayer(string username)
    {
        _logger.LogDebug("Getting player with username={username}.", username);
        return await _context.Players.FirstOrDefaultAsync(p => p.Username == username);
    }

    public async Task<Player> CreatePlayer(Player player)
    {
        _logger.LogDebug("Creating player: {player}.", JsonSerializer.Serialize(player));
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return player;
    }
}