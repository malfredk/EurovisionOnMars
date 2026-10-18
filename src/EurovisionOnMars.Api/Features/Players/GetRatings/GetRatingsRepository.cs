using EurovisionOnMars.Api.DataAccess;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Players.GetRatings;

public interface IGetRatingsRepository
{
    Task<IReadOnlyCollection<PlayerRating>> GetRatingsByPlayerId(int playerId);
}

public class GetRatingsRepository : IGetRatingsRepository
{
    private readonly ILogger<GetRatingsRepository> _logger;
    private readonly DataContext _context;

    public GetRatingsRepository(
        ILogger<GetRatingsRepository> logger,
        DataContext context
        )
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IReadOnlyCollection<PlayerRating>> GetRatingsByPlayerId(int playerId)
    {
        _logger.LogDebug("Getting ratings for player with id={playerId}.", playerId);
        return await _context.PlayerRatings
            .AsNoTracking()
            .Where(r => r.PlayerId == playerId)
            .Include(r => r.Country)
            .Include(r => r.Prediction)
            .ToListAsync();
    }
}