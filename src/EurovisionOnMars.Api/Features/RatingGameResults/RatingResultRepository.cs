using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingResultRepository
{
    Task<ImmutableList<RatingGameResult>> GetRatingResultsForPlayer(int playerId);
    Task<RatingGameResult> UpdateRatingResult(RatingGameResult ratingResult);
}

public class RatingResultRepository : IRatingResultRepository
{
    private readonly DataContext _context;
    private readonly ILogger<RatingResultRepository> _logger;

    public RatingResultRepository(DataContext context, ILogger<RatingResultRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImmutableList<RatingGameResult>> GetRatingResultsForPlayer(int playerId)
    {
        _logger.LogDebug("Getting rating results for player with id={playerId}.", playerId);
        var ratingResults = await _context.Players
            .Where(p => p.Id == playerId)
            .SelectMany(p => p.PlayerRatings)
            .Select(r => r.RatingGameResult)
            .ToListAsync();
        return ratingResults.ToImmutableList();
    }

    public async Task<RatingGameResult> UpdateRatingResult(RatingGameResult ratingResult)
    {
        _logger.LogDebug("Updating rating result: {ratingResult}.", JsonSerializer.Serialize(ratingResult));
        var updatedRatingResult = _context.RatingGameResults.Update(ratingResult);
        await _context.SaveChangesAsync();
        return updatedRatingResult.Entity;
    }
}