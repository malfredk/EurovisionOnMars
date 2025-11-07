using EurovisionOnMars.Api.Features.RatingClosing;
using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IPlayerRatingService
{
    Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings();
    Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId);
    Task UpdatePlayerRating(int id, UpdatePlayerRatingRequestDto ratingRequestDto);
    Task UpdatePlayerRating(int id, int rank);
}

public class PlayerRatingService : IPlayerRatingService
{
    private static List<int> VALID_POINTS = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };
    private static List<int> SPECIAL_POINTS = new List<int>() { 10, 12 };

    private readonly IPlayerRatingRepository _repository;
    private readonly IRatingClosingService _ratingClosingService;
    private readonly ILogger<PlayerRatingService> _logger;

    public PlayerRatingService(
        IPlayerRatingRepository repository,
        IRatingClosingService ratingClosingService,
        ILogger<PlayerRatingService> logger)
    {
        _repository = repository;
        _ratingClosingService = ratingClosingService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings()
    {
        return await _repository.GetAllPlayerRatings();
    }

    public async Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId)
    {
        var ratings = await _repository.GetPlayerRatingsByPlayerId(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"Player with id={playerId} is missing ratings.");
        }
        return SortRatings(ratings);
    }

    public async Task UpdatePlayerRating(int id, UpdatePlayerRatingRequestDto ratingRequestDto)
    {
        _ratingClosingService.ValidateRatingTime();

        var ratings = await _repository.GetPlayerRatingsForPlayer(id);
        var rating = ratings.FirstOrDefault(r => r.Id == id);
        var newCategoryPointsRating = CreateNewPlayerRating(ratingRequestDto);
        
        UpdateCategoryPoints(rating, ratingRequestDto, ratings);
        UpdatePredictions(rating, ratings);
        await SaveUpdatedRatings(ratings);
    }

    public async Task UpdatePlayerRating(int id, int rank)
    {
        _ratingClosingService.ValidateRatingTime();
        ValidateRank(rank);

        var rating = await GetPlayerRating(id);
        UpdateRank(rating, rank);
        await _repository.UpdateRating(rating);
    }

    private ImmutableList<PlayerRating> SortRatings(ImmutableList<PlayerRating> ratings)
    {
        return ratings
            .OrderBy(r => r.Prediction.CalculatedRank ?? 100)
            .ThenBy(r => r.Country.Number)
            .ToImmutableList();
    }

    private async Task<PlayerRating> GetPlayerRating(int id)
    {
        var rating = await _repository.GetRating(id);
        if (rating == null)
        {
            throw new KeyNotFoundException($"No rating with id={id} exists.");
        }
        return rating;
    }

    private PlayerRating CreateNewPlayerRating(UpdatePlayerRatingRequestDto ratingRequestDto)
    {
        return new PlayerRating
        {
            Category1Points = ratingRequestDto.Category1Points,
            Category2Points = ratingRequestDto.Category2Points,
            Category3Points = ratingRequestDto.Category3Points
        };
    }

    private void UpdateCategoryPoints(
        PlayerRating rating,
        UpdatePlayerRatingRequestDto ratingRequest,  
        IReadOnlyList<PlayerRating> ratings
        )
    {
        var newCategoryPointsRating = CreateNewPlayerRating(ratingRequest);
        ValidateCategoryPoints(rating, newCategoryPointsRating, ratings);
        UpdateCategoryPoints(rating, newCategoryPointsRating);
    }

    private void ValidateCategoryPoints(
        PlayerRating rating,
        PlayerRating newCategoryPointsRating, 
        IReadOnlyList<PlayerRating> ratings
        )
    {
        Func<PlayerRating, int?> category1PointsGetter = r => r.Category1Points;
        Func<PlayerRating, int?> category2PointsGetter = r => r.Category2Points;
        Func<PlayerRating, int?> category3PointsGetter = r => r.Category3Points;

        _logger.LogDebug("Validating points in rating with for category 1.");
        ValidatePointsForCategory(rating, newCategoryPointsRating, ratings, category1PointsGetter);

        _logger.LogDebug("Validating points in rating with for category 2.");
        ValidatePointsForCategory(rating, newCategoryPointsRating, ratings, category2PointsGetter);

        _logger.LogDebug("Validating points in rating with for category 3.");
        ValidatePointsForCategory(rating, newCategoryPointsRating, ratings, category3PointsGetter);
    }

    private void ValidatePointsForCategory(
        PlayerRating rating,
        PlayerRating newRating,
        IReadOnlyList<PlayerRating> ratings,
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        if (!HasPointsChanged(rating, newRating, categoryPointsGetter))
        {
            _logger.LogDebug("Skipping validation since points have not changed in this category.");
            return;
        }
        ValidatePoints(newRating, categoryPointsGetter);
        ValidateSpecialPoints(newRating, ratings, categoryPointsGetter);
    }

    private bool HasPointsChanged(
        PlayerRating rating,
        PlayerRating newRating,
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        var points = categoryPointsGetter(rating);
        var newPoints = categoryPointsGetter(newRating);
        return newPoints != points;
    }

    private void ValidatePoints(
        PlayerRating rating, 
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        var categoryPoints = categoryPointsGetter(rating);
        var isValid = categoryPoints != null && VALID_POINTS.Contains((int)categoryPoints);
        if (!isValid)
        {
            throw new ArgumentException("Invalid points amount");
        }
    }

    private void ValidateSpecialPoints
        (
        PlayerRating newRating,
        IReadOnlyList<PlayerRating> ratings,
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        // category points are validated as nonnull in previous validation
        var newPoints = (int)categoryPointsGetter(newRating)!;

        if (!SPECIAL_POINTS.Contains(newPoints))
        {
            return;
        }

        var newPointsCount = ratings
            .Count(r => categoryPointsGetter(r) == newPoints);

        if (newPointsCount > 0)
        {
            _logger.LogInformation("Antoher rating already has {categoryPoints} points in this category.", newPoints);
            throw new ArgumentException("Special points has already been given in category.");
        }
    }

    private void UpdateCategoryPoints(PlayerRating rating, PlayerRating newCategoryPointsRating)
    {
        rating.Category1Points = newCategoryPointsRating.Category1Points;
        rating.Category2Points = newCategoryPointsRating.Category2Points;
        rating.Category3Points = newCategoryPointsRating.Category3Points;
    }

    private void UpdatePredictions(
        PlayerRating rating,
        IReadOnlyList<PlayerRating> ratings
        )
    {
        UpdateTotalPoints(rating);
        UpdateRanks(ratings);
    }

    private void UpdateTotalPoints(PlayerRating rating)
    {
        rating.Prediction.TotalGivenPoints = CalculateTotalPoints(rating);
    }

    private int CalculateTotalPoints(PlayerRating rating)
    {
        return (int)(rating.Category1Points + rating.Category2Points + rating.Category3Points);
    }

    private void UpdateRanks(IReadOnlyList<PlayerRating> ratings)
    {
        var orderedPredicitions = ratings
            .Select(r => r.Prediction)
            .OrderByDescending(p => p.TotalGivenPoints)
            .ToList();

        Prediction? previous = null;
        for (int i = 0; i < orderedPredicitions.Count; i++)
        {
            var current = orderedPredicitions[i];
            if (previous != null && current.TotalGivenPoints == previous.TotalGivenPoints)
            {
                current.CalculatedRank = previous.CalculatedRank;
            }
            else
            {
                current.CalculatedRank = i + 1;
            }
            previous = current;
        }
    }

    private async Task SaveUpdatedRatings(IReadOnlyList<PlayerRating> ratings)
    {
        foreach (var rating in ratings)
        {
            _logger.LogDebug("Updating rating with id={id}.", rating.Id);
            await _repository.UpdateRating(rating);
        }
    }

    private void ValidateRank(int rank)
    {
        if (rank < 1 || rank > 26)
        {
            throw new ArgumentException("Rank must be between 1 and 26");
        }
    }

    private void UpdateRank(PlayerRating entity, int rank)
    {
        entity.Prediction.CalculatedRank = rank;
    }
}