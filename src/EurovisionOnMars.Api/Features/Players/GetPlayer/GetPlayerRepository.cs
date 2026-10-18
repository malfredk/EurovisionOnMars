using EurovisionOnMars.Infrastructure.DataAccess;
using EurovisionOnMars.Domain.Players;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Players.GetPlayer;

public interface IGetPlayerRepository
{
    Task<int?> GetPlayer(Username username);
}

public class GetPlayerRepository : IGetPlayerRepository
{
    private readonly ILogger<GetPlayerRepository> _logger;
    private readonly DataContext _context;

    public GetPlayerRepository(
        ILogger<GetPlayerRepository> logger,
        DataContext context 
        )
    {
        _logger = logger;
        _context = context;
    }

    public async Task<int?> GetPlayer(Username username)
    {
        _logger.LogDebug("Getting player with username={username}.", username);
        var player = await _context.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Username == username);

        return player?.Id;
    }
}