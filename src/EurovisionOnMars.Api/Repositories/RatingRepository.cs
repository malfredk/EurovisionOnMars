﻿using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text.Json;

namespace EurovisionOnMars.Api.Repositories;

public interface IRatingRepository
{
    Task<ImmutableList<Rating>> GetRatingsByPlayer(int playerId);
    Task<Rating?> GetRating(int id);
    Task<Rating> CreateRating(Rating rating);
    Task<Rating> UpdateRating(Rating rating);
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

    public async Task<ImmutableList<Rating>> GetRatingsByPlayer(int playerId)
    {
        _logger.LogDebug("Getting all ratings for player with id={playerId}.", playerId);
        var ratings = await _context.Ratings
            .Where(r => r.PlayerId == playerId)
            .Include(r => r.Country)
            .Include(r => r.RatingResult)
            .ToListAsync();
        return ratings.ToImmutableList();
    }

    public async Task<Rating?> GetRating(int id)
    {
        _logger.LogDebug("Getting rating with id={id}.", id);
        return await _context.Ratings
            .Include(r => r.Country)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Rating> CreateRating(Rating rating)
    {
        _logger.LogDebug("Creating rating: {rating}.", JsonSerializer.Serialize(rating));
        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task<Rating> UpdateRating(Rating rating)
    {
        _logger.LogDebug("Updating rating: {rating}.", JsonSerializer.Serialize(rating));
        var updatedRating = _context.Ratings.Update(rating);
        await _context.SaveChangesAsync();
        return updatedRating.Entity;
    }
}