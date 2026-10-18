using EurovisionOnMars.Infrastructure.DataAccess;
using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.Players.CreatePlayer;

public interface ICreatePlayerRepository
{
    Task<bool> UsernameExists(Username username);
    Task<IReadOnlyCollection<Country>> GetCountries();
    Task<int> AddPlayer(Player player);
}

public class CreatePlayerRepository : ICreatePlayerRepository
{
    private readonly ILogger<CreatePlayerRepository> _logger;
    private readonly DataContext _context;

    public CreatePlayerRepository(
        ILogger<CreatePlayerRepository> logger,
        DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> UsernameExists(Username username)
    {
        _logger.LogDebug(
            "Checking whether player with username={username} exists.",
            username.Value);

        return await _context.Players
            .AsNoTracking()
            .AnyAsync(p => p.Username == username);
    }

    public async Task<IReadOnlyCollection<Country>> GetCountries()
    {
        _logger.LogDebug("Getting countries for new player.");
        return await _context.Countries.ToListAsync();
    }

    public async Task<int> AddPlayer(Player player)
    {
        _logger.LogDebug(
            "Creating player: {player}.",
            JsonSerializer.Serialize(player));

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return player.Id;
    }
}