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
        var rating = ratings.First(r => r.Id == id);

        UpdatePoints(rating, ratingRequestDto, ratings);
        UpdateRanks(ratings);
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

    private void UpdatePoints(
        PlayerRating rating,
        UpdatePlayerRatingRequestDto ratingRequest,  
        IReadOnlyList<PlayerRating> ratings
        )
    {
        rating.SetPoints(
            ratingRequest.Category1Points,
            ratingRequest.Category2Points,
            ratingRequest.Category3Points
            );
        ValidateSpecialCategoryPoints(rating, ratings);
    }

    private void ValidateSpecialCategoryPoints(
        PlayerRating rating,
        IReadOnlyList<PlayerRating> ratings
        )
    {
        Func<PlayerRating, int?> category1PointsGetter = r => r.Category1Points;
        Func<PlayerRating, int?> category2PointsGetter = r => r.Category2Points;
        Func<PlayerRating, int?> category3PointsGetter = r => r.Category3Points;

        _logger.LogDebug("Validating points in rating with for category 1.");
        ValidateSpecialCategoryPoints(rating, ratings, category1PointsGetter);

        _logger.LogDebug("Validating points in rating with for category 2.");
        ValidateSpecialCategoryPoints(rating, ratings, category2PointsGetter);

        _logger.LogDebug("Validating points in rating with for category 3.");
        ValidateSpecialCategoryPoints(rating, ratings, category3PointsGetter);
    }

    private void ValidateSpecialCategoryPoints(
        PlayerRating rating,
        IReadOnlyList<PlayerRating> ratings,
        Func<PlayerRating, int?> categoryPointsGetter
        )
    {
        var points = (int)categoryPointsGetter(rating)!;
        if (!PlayerRating.SPECIAL_POINTS.Contains(points))
        {
            return;
        }

        var samePointsCount = ratings
            .Count(r => categoryPointsGetter(r) == points);

        if (samePointsCount > 1)
        {
            throw new ArgumentException("Special points have already been given in category.");
        }
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