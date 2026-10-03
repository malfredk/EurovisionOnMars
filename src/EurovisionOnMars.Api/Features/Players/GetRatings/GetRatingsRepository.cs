using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.Players.GetRatings;

public interface IGetRatingsRepository
{
    Task<ImmutableList<PlayerRating>> GetRatingsByPlayerId(int playerId);
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

    public async Task<ImmutableList<PlayerRating>> GetRatingsByPlayerId(int playerId)
    {
        _logger.LogDebug("Getting ratings for player with id={playerId}.", playerId);
        var ratings = await _context.PlayerRatings
            .AsNoTracking()
            .Where(r => r.PlayerId == playerId)
            .Include(r => r.Country)
            .Include(r => r.Prediction)
            .ToListAsync();
        return ratings.ToImmutableList();
    }
}