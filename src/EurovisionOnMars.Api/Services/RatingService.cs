using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Dto.Requests;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Services;

public interface IRatingService
{
    Task<ImmutableList<PlayerRating>> GetRatingsByPlayer(int playerId);
    Task<PlayerRating> GetRating(int id);
    Task UpdateRating(int id, RatingPointsRequestDto ratingRequestDto);
    Task UpdateRating(int id, int rank);
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

    public async Task<ImmutableList<PlayerRating>> GetRatingsByPlayer(int playerId)
    {
        var ratings = await _repository.GetRatingsByPlayer(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"Player with id={playerId} is missing ratings");
        }
        return SortRatingsByRankThenNumber(ratings);
    }
    
    public async Task<PlayerRating> GetRating(int id)
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
        var oldRank = existingRating.Prediction.CalculatedRank;
        var oldPointsSum = existingRating.Prediction.TotalGivenPoints;

        var ratings = await GetRatingsByPlayer(existingRating.PlayerId);

        var updatedRating = UpdateEntity(existingRating, ratingRequestDto);

        ValidatePoints(updatedRating, ratings);
        await SetRanksAndUpdateDatabase(oldRank, oldPointsSum, updatedRating, ratings);
    }

    public async Task UpdateRating(int id, int rank)
    {
        _ratingClosingService.ValidateRatingTime();
        ValidateRank(rank);

        var existingRating = await GetRating(id);
        var updatedRating = UpdateEntity(existingRating, rank);
        await _repository.UpdateRating(updatedRating);
    }

    private void ValidateRank(int rank)
    {
        if (rank < 1 || rank > 26)
        {
            throw new ArgumentException("Rank must be between 1 and 26");
        }
    }

    private PlayerRating UpdateEntity(PlayerRating entity, int rank)
    {
        entity.Prediction.CalculatedRank = rank;
        return entity;
    }

    private PlayerRating UpdateEntity(PlayerRating entity, RatingPointsRequestDto dto)
    {
        entity.Category1Points = dto.Category1Points;
        entity.Category2Points = dto.Category2Points;
        entity.Category3Points = dto.Category3Points;
        entity.Prediction.TotalGivenPoints = dto.Category1Points + dto.Category2Points + dto.Category3Points;
        return entity;
    }

    private ImmutableList<PlayerRating> SortRatingsByRankThenNumber(ImmutableList<PlayerRating> ratings)
    {
        return ratings
            .OrderBy(r => r.Prediction.CalculatedRank ?? 100)
            .ThenBy(r => r.Country.Number)
            .ToImmutableList();
    }

    private async Task SetRanksAndUpdateDatabase(
        int? oldRank,
        int? oldPointsSum,
        PlayerRating updatedPointsRating,
        ImmutableList<PlayerRating> ratingsWithUpdatedPoints
        )
    {
        var selectedRatingId = updatedPointsRating.Id;
        int updatedPointsSum = updatedPointsRating.Prediction.TotalGivenPoints ?? throw new Exception("Updated points sum cannot be null");
        // no need to update ranks if points sum is unchanged
        if (oldPointsSum == updatedPointsSum)
        {
            await _repository.UpdateRating(updatedPointsRating);
            return;
        }

        oldRank = CalculateOldRank(oldRank, oldPointsSum, ratingsWithUpdatedPoints, selectedRatingId);
        int updatedRank = CalculateRank(updatedPointsSum, selectedRatingId, SortByPoints(ratingsWithUpdatedPoints));

        var rankDifference = CalculateRankDifference(oldRank, updatedRank);

        int minRankToUpdate;
        int maxRankToUpdate;
        if (oldRank == null)
        {
            minRankToUpdate = updatedRank;
            maxRankToUpdate = 26;
        }
        else
        {
            minRankToUpdate = Math.Min((int)oldRank, updatedRank);
            maxRankToUpdate = Math.Max((int)oldRank, updatedRank);
        }

        foreach (var rating in ratingsWithUpdatedPoints)
        {
            var pointsSum = rating.Prediction.TotalGivenPoints;
            // overwrite rank for the selected rating
            if (rating.Id == selectedRatingId)
            {
                rating.Prediction.CalculatedRank = updatedRank;
            }
            // no need to update null rank
            else if (pointsSum == null)
            {
                continue;
            }
            // overwrite rank for ratings with the same new points sum
            else if (pointsSum == updatedPointsSum)
            {
                rating.Prediction.CalculatedRank = updatedRank;
            }
            // reset rank for ratings with the same old points sum
            else if (pointsSum == oldPointsSum)
            {
                if (rankDifference == RankDifference.NEGATIVE)
                {
                    rating.Prediction.CalculatedRank = oldRank;
                }
                else
                {
                    rating.Prediction.CalculatedRank = oldRank + 1;
                }
            }
            // push the effected ratings one up or one down
            else if (rating.Prediction.CalculatedRank >= minRankToUpdate &&
                rating.Prediction.CalculatedRank <= maxRankToUpdate)
            {
                rating.Prediction.CalculatedRank += ((int)rankDifference);
            }
            else
            {
                continue;
            }

            await _repository.UpdateRating(rating);
        }
    }

    private int? CalculateOldRank(
        int? oldRank, 
        int? oldPointsSum, 
        ImmutableList<PlayerRating> ratingsWithUpdatedPoints, 
        int selectedRatingId
        )
    {
        if (oldRank is null)
        {
            return oldRank;
        }

        if (oldPointsSum is null)
        {
            throw new Exception("Old rank is nonnull, thus old points sum should be nonnull");
        }

        // recalculating old rank in case it has been overwritten
        return CalculateRank
            (oldPointsSum,
            selectedRatingId,
            SortByPointsConsiderOldPoints((int)oldPointsSum, selectedRatingId, ratingsWithUpdatedPoints));
    }

    private RankDifference CalculateRankDifference(int? oldRank, int newRank)
    {
        if (oldRank == null || newRank < oldRank)
        {
            return RankDifference.POSITIVE;
        }
        else if (newRank == oldRank)
        {
            return RankDifference.NONE;
        }
        return RankDifference.NEGATIVE;
    }

    private enum RankDifference
    {
        POSITIVE = 1,
        NEGATIVE = -1,
        NONE = 0
    }

    private List<PlayerRating> SortByPointsConsiderOldPoints(
        int oldPointsSum,
        int selectedRatingId,
        ImmutableList<PlayerRating> ratings
        )
    {
        return ratings
            .OrderByDescending(r => r.Id == selectedRatingId ? oldPointsSum : (r.Prediction.TotalGivenPoints ?? 0))
            .ToList();
    }

    private List<PlayerRating> SortByPoints(ImmutableList<PlayerRating> ratings)
    {
        return ratings
            .OrderByDescending(r => r.Prediction.TotalGivenPoints ?? 0)
            .ToList();
    }

    private int CalculateRank
        (
        int? selectedPointsSum,
        int selectedRatingId, 
        List<PlayerRating> ratingsSortedByDescendigPoints
        )
    {
        int? previousPoints = -1; // initiated to ensure it is different from currentPoints the first iteration
        int? currentPoints;
        int rank = 0;
        int sameRankCount = 1;
        foreach (var rating in ratingsSortedByDescendigPoints)
        {
            if (rating.Id == selectedRatingId)
            {
                currentPoints = selectedPointsSum;
            }
            else
            {
                currentPoints = rating.Prediction.TotalGivenPoints;
            }

            if (currentPoints == previousPoints)
            {
                sameRankCount++;
            }
            else
            {

                rank += sameRankCount;
                sameRankCount = 1;
            }

            if (rating.Id == selectedRatingId)
            {
                return rank;
            }
            previousPoints = currentPoints;
        }
        throw new Exception("Error in rank calculation");
    }

    private void ValidatePoints(PlayerRating rating, ImmutableList<PlayerRating> existingRatings)
    {
        Func<PlayerRating, int?> category1PointsGetter = r => r.Category1Points;
        Func<PlayerRating, int?> category2PointsGetter = r => r.Category2Points;
        Func<PlayerRating, int?> category3PointsGetter = r => r.Category3Points;

        var id = rating.Id;
        _logger.LogDebug("Validating points in rating with id={id} for category 1.", id);
        ValidatePointsForCategory(rating, existingRatings, category1PointsGetter);
        _logger.LogDebug("Validating points in rating with id={id} for category 2.", id);
        ValidatePointsForCategory(rating, existingRatings, category2PointsGetter);
        _logger.LogDebug("Validating points in rating with id={id} for category 3.", id);
        ValidatePointsForCategory(rating, existingRatings, category3PointsGetter);
    }

    private void ValidatePointsForCategory(
        PlayerRating rating,
        ImmutableList<PlayerRating> existingRatings,
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        ValidatePointsAmountForCategory(rating, categoryPointsGetter);
        ValidateSpecialPointsForCategory(rating, existingRatings, categoryPointsGetter);
    }

    private void ValidatePointsAmountForCategory(PlayerRating rating, Func<PlayerRating, int?> categoryPointsGetter)
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
        PlayerRating rating, 
        ImmutableList<PlayerRating> existingRatings, 
        Func<PlayerRating, int?> categoryPointsGetter
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