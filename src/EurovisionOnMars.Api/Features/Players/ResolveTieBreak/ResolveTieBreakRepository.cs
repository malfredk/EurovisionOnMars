using EurovisionOnMars.Domain.DataAccess;
using EurovisionOnMars.Domain.Players;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Players.ResolveTieBreak;

public interface IResolveTieBreakRepository
{
    Task<Player?> GetPlayer(int playerId);
    Task SaveChanges();
}

public class ResolveTieBreakRepository : IResolveTieBreakRepository
{
    private readonly ILogger<ResolveTieBreakRepository> _logger;
    private readonly DataContext _dataContext;

    public ResolveTieBreakRepository(
        ILogger<ResolveTieBreakRepository> logger,
        DataContext dataContext
        )
    {
        _logger = logger;
        _dataContext = dataContext;
    }

    public async Task<Player?> GetPlayer(int playerId)
    {
        return await _dataContext.Players
            .Include(p => p.PlayerRatings)
                .ThenInclude(r => r.Prediction)
            .SingleOrDefaultAsync(p => p.Id == playerId);
    }

    public async Task SaveChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
