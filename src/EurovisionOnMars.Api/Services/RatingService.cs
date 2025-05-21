using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Dto.Requests;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Services;

public interface IRatingService
{
    Task<ImmutableList<Rating>> GetRatingsByPlayer(int playerId);
    Task<Rating> GetRating(int id);
    Task UpdateRating(int id, RatingPointsRequestDto ratingRequestDto);
    Task UpdateRating(int id, int ranking);
}

public class RatingService : IRatingService
{
    private static List<int> VALID_POINTS = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };
    private static List<int> SPECIAL_POINTS = new List<int>() { 10, 12 };

    private readonly IRatingRepository _repository;
    private readonly IRatingClosingService _ratingClosingService;
    private readonly ILogger<RatingService> _logger;

    public RatingService(
        IRatingRepository repository,
        IRatingClosingService ratingClosingService,
        ILogger<RatingService> logger)
    {
        _repository = repository;
        _ratingClosingService = ratingClosingService;
        _logger = logger;
    }

    public async Task<ImmutableList<Rating>> GetRatingsByPlayer(int playerId)
    {
        var ratings = await _repository.GetRatingsByPlayer(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"Player with id={playerId} is missing ratings");
        }
        return SortRatingsByRankingThenNumber(ratings);
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

    public async Task UpdateRating(int id, RatingPointsRequestDto ratingRequestDto)
    {
        _ratingClosingService.ValidateRatingTime();

        var existingRating = await GetRating(id);
        var oldRanking = existingRating.Ranking;
        var oldPointsSum = existingRating.PointsSum;

        var ratings = await GetRatingsByPlayer(existingRating.PlayerId);

        var updatedRating = UpdateEntity(existingRating, ratingRequestDto);

        ValidatePoints(updatedRating, ratings);
        await SetRankingsAndUpdateDatabase(oldRanking, oldPointsSum, updatedRating, ratings);
    }

    public async Task UpdateRating(int id, int ranking)
    {
        _ratingClosingService.ValidateRatingTime();
        ValidateRanking(ranking);

        var existingRating = await GetRating(id);
        var updatedRating = UpdateEntity(existingRating, ranking);
        await _repository.UpdateRating(updatedRating);
    }

    private void ValidateRanking(int ranking)
    {
        if (ranking < 1 || ranking > 26)
        {
            throw new ArgumentException("Ranking must be between 1 and 26");
        }
    }

    private Rating UpdateEntity(Rating entity, int ranking)
    {
        entity.Ranking = ranking;
        return entity;
    }

    private Rating UpdateEntity(Rating entity, RatingPointsRequestDto dto)
    {
        entity.Category1Points = dto.Category1Points;
        entity.Category2Points = dto.Category2Points;
        entity.Category3Points = dto.Category3Points;
        entity.PointsSum = dto.Category1Points + dto.Category2Points + dto.Category3Points;
        return entity;
    }

    private ImmutableList<Rating> SortRatingsByRankingThenNumber(ImmutableList<Rating> ratings)
    {
        return ratings
            .OrderBy(r => r.Ranking ?? 100)
            .ThenBy(r => r.Country.Number)
            .ToImmutableList();
    }

    private async Task SetRankingsAndUpdateDatabase(
        int? oldRanking,
        int? oldPointsSum,
        Rating updatedPointsRating,
        ImmutableList<Rating> ratingsWithUpdatedPoints
        )
    {
        var selectedRatingId = updatedPointsRating.Id;
        int updatedPointsSum = updatedPointsRating.PointsSum ?? throw new Exception("Updated points sum cannot be null");
        // no need to update rankings if points sum is unchanged
        if (oldPointsSum == updatedPointsSum)
        {
            await _repository.UpdateRating(updatedPointsRating);
            return;
        }

        oldRanking = CalculateOldRanking(oldRanking, oldPointsSum, ratingsWithUpdatedPoints, selectedRatingId);
        int updatedRanking = CalculateRanking(updatedPointsSum, selectedRatingId, SortByPoints(ratingsWithUpdatedPoints));

        var rankingDifference = CalculateRankingDifference(oldRanking, updatedRanking);

        int minRankingToUpdate;
        int maxRankingToUpdate;
        if (oldRanking == null)
        {
            minRankingToUpdate = updatedRanking;
            maxRankingToUpdate = 26;
        }
        else
        {
            minRankingToUpdate = Math.Min((int)oldRanking, updatedRanking);
            maxRankingToUpdate = Math.Max((int)oldRanking, updatedRanking);
        }

        foreach (var rating in ratingsWithUpdatedPoints)
        {
            var pointsSum = rating.PointsSum;
            // overwrite ranking for the selected rating
            if (rating.Id == selectedRatingId)
            {
                rating.Ranking = updatedRanking;
            }
            // no need to update null ranking
            else if (pointsSum == null)
            {
                continue;
            }
            // overwrite ranking for ratings with the same new points sum
            else if (pointsSum == updatedPointsSum)
            {
                rating.Ranking = updatedRanking;
            }
            // reset ranking for ratings with the same old points sum
            else if (pointsSum == oldPointsSum)
            {
                if (rankingDifference == RankingDifference.NEGATIVE)
                {
                    rating.Ranking = oldRanking;
                }
                else
                {
                    rating.Ranking = oldRanking + 1;
                }
            }
            // push the effected ratings one up or one down
            else if (rating.Ranking >= minRankingToUpdate &&
                rating.Ranking <= maxRankingToUpdate)
            {
                rating.Ranking += ((int)rankingDifference);
            }
            else
            {
                continue;
            }

            await _repository.UpdateRating(rating);
        }
    }

    private int? CalculateOldRanking(
        int? oldRanking, 
        int? oldPointsSum, 
        ImmutableList<Rating> ratingsWithUpdatedPoints, 
        int selectedRatingId
        )
    {
        if (oldRanking is null)
        {
            return oldRanking;
        }

        if (oldPointsSum is null)
        {
            throw new Exception("Old ranking is nonnull, thus old points sum should be nonnull");
        }

        // recalculating old ranking in case it has been overwritten
        return CalculateRanking
            (oldPointsSum,
            selectedRatingId,
            SortByPointsConsiderOldPoints((int)oldPointsSum, selectedRatingId, ratingsWithUpdatedPoints));
    }

    private RankingDifference CalculateRankingDifference(int? oldRanking, int newRanking)
    {
        if (oldRanking == null || newRanking < oldRanking)
        {
            return RankingDifference.POSITIVE;
        }
        else if (newRanking == oldRanking)
        {
            return RankingDifference.NONE;
        }
        return RankingDifference.NEGATIVE;
    }

    private enum RankingDifference
    {
        POSITIVE = 1,
        NEGATIVE = -1,
        NONE = 0
    }

    private List<Rating> SortByPointsConsiderOldPoints(
        int oldPointsSum,
        int selectedRatingId,
        ImmutableList<Rating> ratings
        )
    {
        return ratings
            .OrderByDescending(r => r.Id == selectedRatingId ? oldPointsSum : (r.PointsSum ?? 0))
            .ToList();
    }

    private List<Rating> SortByPoints(ImmutableList<Rating> ratings)
    {
        return ratings
            .OrderByDescending(r => r.PointsSum ?? 0)
            .ToList();
    }

    private int CalculateRanking
        (
        int? selectedPointsSum,
        int selectedRatingId, 
        List<Rating> ratingsSortedByDescendigPoints
        )
    {
        int? previousPoints = -1; // initiated to ensure it is different from currentPoints the first iteration
        int? currentPoints;
        int ranking = 0;
        int sameRankingCount = 1;
        foreach (var rating in ratingsSortedByDescendigPoints)
        {
            if (rating.Id == selectedRatingId)
            {
                currentPoints = selectedPointsSum;
            }
            else
            {
                currentPoints = rating.PointsSum;
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

            if (rating.Id == selectedRatingId)
            {
                return ranking;
            }
            previousPoints = currentPoints;
        }
        throw new Exception("Error in ranking calculation");
    }

    private void ValidatePoints(Rating rating, ImmutableList<Rating> existingRatings)
    {
        Func<Rating, int?> category1PointsGetter = r => r.Category1Points;
        Func<Rating, int?> category2PointsGetter = r => r.Category2Points;
        Func<Rating, int?> category3PointsGetter = r => r.Category3Points;

        var id = rating.Id;
        _logger.LogDebug("Validating points in rating with id={id} for category 1.", id);
        ValidatePointsForCategory(rating, existingRatings, category1PointsGetter);
        _logger.LogDebug("Validating points in rating with id={id} for category 2.", id);
        ValidatePointsForCategory(rating, existingRatings, category2PointsGetter);
        _logger.LogDebug("Validating points in rating with id={id} for category 3.", id);
        ValidatePointsForCategory(rating, existingRatings, category3PointsGetter);
    }

    private void ValidatePointsForCategory(
        Rating rating,
        ImmutableList<Rating> existingRatings,
        Func<Rating, int?> categoryPointsGetter
        )
    {
        ValidatePointsAmountForCategory(rating, categoryPointsGetter);
        ValidateSpecialPointsForCategory(rating, existingRatings, categoryPointsGetter);
    }

    private void ValidatePointsAmountForCategory(Rating rating, Func<Rating, int?> categoryPointsGetter)
    {
        var categoryPoints = categoryPointsGetter(rating);
        var isValid = categoryPoints != null && VALID_POINTS.Contains((int)categoryPoints);
        if (!isValid)
        {
            throw new ArgumentException("Invalid points amount");
        }
    }

    private void ValidateSpecialPointsForCategory
        (
        Rating rating, 
        ImmutableList<Rating> existingRatings, 
        Func<Rating, int?> categoryPointsGetter
        )
    {
        // category points are validated as nonnull in previous validation
        var categoryPoints = (int)categoryPointsGetter(rating)!;

        if (!SPECIAL_POINTS.Contains(categoryPoints))
        {
            return;
        }

        foreach (var existingRating in existingRatings)
        {
            if (existingRating.Id ==  rating.Id)
            {
                continue;
            }
            
            if (categoryPointsGetter(existingRating) == categoryPoints)
            {
                _logger.LogWarning("Antoher rating, with id={id}, already has {categoryPoints} points in this category.", existingRating.Id, categoryPoints);
                throw new ArgumentException("Special points has already been given in category.");
            }
        }
    }
}