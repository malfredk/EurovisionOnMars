using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IPlayerRatingRepository
{
    Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings();
    Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId);
    Task<IReadOnlyList<PlayerRating>> GetPlayerRatingsForPlayer(int id);
    Task<PlayerRating?> GetRating(int id);
    Task<PlayerRating> UpdateRating(PlayerRating rating);
}

public class PlayerRatingRepository : IPlayerRatingRepository
{
    private readonly DataContext _context;
    private readonly ILogger<PlayerRatingRepository> _logger;

    public PlayerRatingRepository(DataContext context, ILogger<PlayerRatingRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings()
    {
        _logger.LogDebug("Getting all ratings.");
        return await _context.PlayerRatings
            .Include(pr => pr.Country)
            .Include(pr => pr.RatingGameResult)
            .Include(pr => pr.Prediction)
            .ToListAsync();
    }

    public async Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId)
    {
        _logger.LogDebug("Getting ratings for player with id={playerId}.", playerId);
        var ratings = await _context.PlayerRatings
            .Where(r => r.PlayerId == playerId)
            .Include(r => r.Country)
            .Include(r => r.Prediction)
            .ToListAsync();
        return ratings.ToImmutableList();
    }

    public async Task<IReadOnlyList<PlayerRating>> GetPlayerRatingsForPlayer(int id)
    {
        _logger.LogDebug("Getting ratings with same playerId as rating with id={id}", id);
        return await _context.PlayerRatings
            .Where(r => r.Id == id)
            .SelectMany(r => r.Player.PlayerRatings)
            .Include(r => r.Prediction)
            .ToListAsync();
    }

    public async Task<PlayerRating?> GetRating(int id)
    {
        _logger.LogDebug("Getting rating with id={id}.", id);
        return await _context.PlayerRatings
            .Include(r => r.Country)
            .Include(r => r.Prediction)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<PlayerRating> UpdateRating(PlayerRating rating)
    {
        _logger.LogDebug("Updating rating: {rating}.", JsonSerializer.Serialize(rating));
        var updatedRating = _context.PlayerRatings.Update(rating);
        await _context.SaveChangesAsync();
        return updatedRating.Entity;
    }
}