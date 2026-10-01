using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.Players.GetRatingGameResults;

public interface IGetRatingGameResultsRepository
{
    Task<ImmutableList<PlayerRating>> GetRatings(int playerId);
}

public class GetRatingGameResultsRepository : IGetRatingGameResultsRepository
{
    private readonly ILogger<GetRatingGameResultsRepository> _logger;
    private readonly DataContext _context;

    public GetRatingGameResultsRepository(
        ILogger<GetRatingGameResultsRepository> logger, 
        DataContext context 
        )
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ImmutableList<PlayerRating>> GetRatings(int playerId)
    {
        _logger.LogDebug("Getting ratings for player with id={playerId}.", playerId);
        var results = await _context.PlayerRatings
            .AsNoTracking()
            .Where(r => r.PlayerId == playerId)
            .Include(r => r.RatingGameResult)
            .Include(r => r.Country)
            .ToListAsync();
        return results.ToImmutableList();
    }
}