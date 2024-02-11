using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Services;

public interface IRatingService
{
    Task<ImmutableList<Rating>> GetRatingsByPlayer(int playerId);
    Task<Rating> GetRating(int id);
    Task<Rating> UpdateRating(Rating rating);
}

public class RatingService : IRatingService
{
    private static List<int> VALID_POINTS = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };
    private static List<int> SPECIAL_POINTS = new List<int>() { 10, 12 };

    private readonly IRatingRepository _repository;
    private readonly ILogger<RatingService> _logger;

    public RatingService(IRatingRepository repository, ILogger<RatingService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ImmutableList<Rating>> GetRatingsByPlayer(int playerId)
    {
        var ratings = await _repository.GetRatingsByPlayer(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"Player with id={playerId} is missing ratings");
        }
        return ratings;
    }
    
    public async Task<Rating> GetRating(int id)
    {
        var rating = await _repository.GetRating(id);
        if (rating == null)
        {
            throw new KeyNotFoundException($"No rating with id={id} exists");
        }
        return rating;
    }

    public async Task<Rating> UpdateRating(Rating rating)
    {
        var ratings = await GetRatingsByPlayer(rating.PlayerId);
        ValidatePoints(rating, ratings);

        return await _repository.UpdateRating(rating);
    }

    private void ValidatePoints(Rating rating, ImmutableList<Rating> existingRatings)
    {
        Func<Rating, int?> category1PointsGetter = r => r.Category1;
        Func<Rating, int?> category2PointsGetter = r => r.Category2;
        Func<Rating, int?> category3PointsGetter = r => r.Category3;

        _logger.LogDebug($"Validating points in rating with id={rating.Id} for category 1");
        ValidatePointsForCategory(rating, existingRatings, category1PointsGetter);
        _logger.LogDebug($"Validating points in rating with id={rating.Id} for category 2");
        ValidatePointsForCategory(rating, existingRatings, category2PointsGetter);
        _logger.LogDebug($"Validating points in rating with id={rating.Id} for category 3");
        ValidatePointsForCategory(rating, existingRatings, category3PointsGetter);
    }

    private void ValidatePointsForCategory(
        Rating rating,
        ImmutableList<Rating> existingRatings,
        Func<Rating, int?> categoryPointsGetter
        )
    {
        if (!ValidatePointsAmountForCategory(rating, categoryPointsGetter))
        {
            throw new ArgumentException("Invalid points amount");
        }

        if (!ValidateSpecialPointsForCategory(rating, existingRatings, categoryPointsGetter))
        {
            throw new ArgumentException("Special points already given in category");
        }
    }

    private bool ValidatePointsAmountForCategory(Rating rating, Func<Rating, int?> categoryPointsGetter)
    {
        var categoryPoints = categoryPointsGetter(rating);
        return categoryPoints != null && VALID_POINTS.Contains((int)categoryPoints);
    }

    private bool ValidateSpecialPointsForCategory
        (
        Rating rating, 
        ImmutableList<Rating> existingRatings, 
        Func<Rating, int?> categoryPointsGetter
        )
    {
        var categoryPoints = (int)categoryPointsGetter(rating);

        if (!SPECIAL_POINTS.Contains(categoryPoints))
        {
            return true;
        }

        foreach (var existingRating in existingRatings)
        {
            if (existingRating.Id ==  rating.Id)
            {
                continue;
            }
            
            if (categoryPointsGetter(existingRating) == categoryPoints)
            {
                _logger.LogDebug($"Rating with id={existingRating.Id} already has {categoryPoints} points");
                return false;
            }
        }

        return true;
    }
}