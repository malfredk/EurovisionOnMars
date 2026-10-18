using EurovisionOnMars.Api.DataAccess;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Features.Players.GetRatingGameResults;

public interface IGetRatingGameResultsRepository
{
    Task<IReadOnlyCollection<PlayerRating>> GetRatings(int playerId);
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

    public async Task<IReadOnlyCollection<PlayerRating>> GetRatings(int playerId)
    {
        _logger.LogDebug("Getting ratings for player with id={playerId}.", playerId);
        return await _context.PlayerRatings
            .AsNoTracking()
            .Where(r => r.PlayerId == playerId)
            .Include(r => r.RatingGameResult)
            .Include(r => r.Country)
            .ToListAsync();
    }
}