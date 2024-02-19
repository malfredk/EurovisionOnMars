using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Services;

public interface IRatingService
{
    Task<ImmutableList<Rating>> GetRatingsByPlayer(int playerId);
    Task<Rating> GetRating(int id);
    Task UpdateRating(Rating rating);
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

    public async Task UpdateRating(Rating newRating)
    {
        var ratings = await GetRatingsByPlayer(newRating.PlayerId);

        ValidatePoints(newRating, ratings);
        ratings = ReplaceRatingInList(newRating, ratings);
        ratings = SetRankings(ratings);

        foreach (var rating in ratings)
        {
            await _repository.UpdateRating(rating);
        }
    }

    private ImmutableList<Rating> ReplaceRatingInList(Rating newRating, ImmutableList<Rating> ratings)
    {
        var oldRating = ratings.FirstOrDefault(r => r.Id == newRating.Id);
        return ratings.Replace(oldRating!, newRating);
    }

    private ImmutableList<Rating> SetRankings(ImmutableList<Rating> ratings)
    {
        var sortedRatings = ratings.Sort((r1, r2) => (r2.PointsSum ?? 0).CompareTo(r1.PointsSum ??  0))
            .ToList();

        int? previousPoints = -1; // initiated to ensure first currentPoints is different
        int? currentPoints;
        int ranking = 0;
        int sameRankingCount = 1;
        foreach (var rating in sortedRatings)
        {
            currentPoints = rating.PointsSum;

            // will not set ranking for countries that have not been voted on yet
            if (currentPoints == null)
            {
                break;
            }

            if (currentPoints == previousPoints)
            {
                sameRankingCount++;
            }
            else
            {

                ranking += sameRankingCount;
                sameRankingCount = 1;
            }

            rating.Ranking = ranking;
            previousPoints = currentPoints;
        }
        return sortedRatings.ToImmutableList();
    }

    private void ValidatePoints(Rating rating, ImmutableList<Rating> existingRatings)
    {
        Func<Rating, int?> category1PointsGetter = r => r.Category1Points;
        Func<Rating, int?> category2PointsGetter = r => r.Category2Points;
        Func<Rating, int?> category3PointsGetter = r => r.Category3Points;

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