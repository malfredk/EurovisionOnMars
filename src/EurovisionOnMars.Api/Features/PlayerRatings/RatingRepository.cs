using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IRatingRepository
{
    Task<ImmutableList<PlayerRating>> GetRatingsByPlayer(int playerId);
    Task<PlayerRating?> GetRating(int id);
    Task<PlayerRating> CreateRating(PlayerRating rating);
    Task<PlayerRating> UpdateRating(PlayerRating rating);
}

public class RatingRepository : IRatingRepository
{
    private readonly DataContext _context;
    private readonly ILogger<RatingRepository> _logger;

    public RatingRepository(DataContext context, ILogger<RatingRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImmutableList<PlayerRating>> GetRatingsByPlayer(int playerId)
    {
        _logger.LogDebug("Getting all ratings for player with id={playerId}.", playerId);
        var ratings = await _context.PlayerRatings
            .Where(r => r.PlayerId == playerId)
            .Include(r => r.Country)
            .Include(r => r.RatingGameResult)
            .Include(r => r.Prediction)
            .ToListAsync();
        return ratings.ToImmutableList();
    }

    public async Task<PlayerRating?> GetRating(int id)
    {
        _logger.LogDebug("Getting rating with id={id}.", id);
        return await _context.PlayerRatings
            .Include(r => r.Country)
            .Include(r => r.Prediction)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<PlayerRating> CreateRating(PlayerRating rating)
    {
        _logger.LogDebug("Creating rating: {rating}.", JsonSerializer.Serialize(rating));
        _context.PlayerRatings.Add(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task<PlayerRating> UpdateRating(PlayerRating rating)
    {
        _logger.LogDebug("Updating rating: {rating}.", JsonSerializer.Serialize(rating));
        var updatedRating = _context.PlayerRatings.Update(rating);
        await _context.SaveChangesAsync();
        return updatedRating.Entity;
    }
}