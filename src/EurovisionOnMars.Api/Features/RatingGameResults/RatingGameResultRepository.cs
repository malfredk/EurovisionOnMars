using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultRepository
{
    Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId);
    Task<RatingGameResult> UpdateRatingGameResult(RatingGameResult ratingResult);
}

public class RatingGameResultRepository : IRatingGameResultRepository
{
    private readonly DataContext _context;
    private readonly ILogger<RatingGameResultRepository> _logger;

    public RatingGameResultRepository(DataContext context, ILogger<RatingGameResultRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId)
    {
        _logger.LogDebug("Getting rating results for player with id={playerId}.", playerId);
        var ratingResults = await _context.RatingGameResults
            .Where(rgr => rgr.PlayerRating.PlayerId == playerId)
            .Include(rgr => rgr.PlayerRating)
            .Include(rgr => rgr.PlayerRating.Country)
            .ToListAsync();
        return ratingResults.ToImmutableList();
    }

    public async Task<RatingGameResult> UpdateRatingGameResult(RatingGameResult ratingResult)
    {
        _logger.LogDebug("Updating rating result: {ratingResult}.", JsonSerializer.Serialize(ratingResult));
        var updatedRatingResult = _context.RatingGameResults.Update(ratingResult);
        await _context.SaveChangesAsync();
        return updatedRatingResult.Entity;
    }
}