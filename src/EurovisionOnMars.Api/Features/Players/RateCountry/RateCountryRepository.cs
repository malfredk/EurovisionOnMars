using EurovisionOnMars.Entity.DataAccess;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Features.Players.RateCountry;

public interface IRateCountryRepository
{
    Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings();
    Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId);
    Task<IReadOnlyList<PlayerRating>> GetPlayerRatingsForPlayer(int id);
    Task SaveChanges();
}

public class RateCountryRepository : IRateCountryRepository
{
    private readonly DataContext _context;
    private readonly ILogger<RateCountryRepository> _logger;

    public RateCountryRepository(DataContext context, ILogger<RateCountryRepository> logger)
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
        _logger.LogDebug("Getting ratings under same player as rating with id={id}", id);
        return await _context.PlayerRatings
            .Where(r => r.Id == id)
            .SelectMany(r => r.Player.PlayerRatings)
            .Include(r => r.Prediction)
            .ToListAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}