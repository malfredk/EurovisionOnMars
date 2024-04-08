using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Repositories;

public interface IRatingResultRepository
{
    Task<ImmutableList<RatingResult>> GetRatingResultsForPlayer(int playerId);
    Task<RatingResult> UpdateRatingResult(RatingResult ratingResult);
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

    public async Task<ImmutableList<RatingResult>> GetRatingResultsForPlayer(int playerId)
    {
        _logger.LogDebug("Getting rating results for player with id={playerId}.", playerId);
        var ratingResults = await _context.Players
            .Where(p => p.Id == playerId)
            .SelectMany(p => p.Ratings)
            .Select(r => r.RatingResult)
            .ToListAsync();
        return ratingResults.ToImmutableList();
    }

    public async Task<RatingResult> UpdateRatingResult(RatingResult ratingResult)
    {
        _logger.LogDebug("Updating rating result: {ratingResult}.", JsonSerializer.Serialize(ratingResult));
        var updatedRatingResult = _context.RatingResults.Update(ratingResult);
        await _context.SaveChangesAsync();
        return updatedRatingResult.Entity;
    }
}