using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IPlayerRatingService
{
    Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings();
    Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId);
    Task UpdatePlayerRating(int id, UpdatePlayerRatingRequestDto ratingRequestDto);
}

public class PlayerRatingService : IPlayerRatingService
{
    private readonly IPlayerRatingRepository _repository;
    private readonly IRatingTimeValidator _ratingTimeValidator;
    private readonly ILogger<PlayerRatingService> _logger;
    private readonly ISpecialPointsValidator _specialPointsValidator;
    private readonly IRankHandler _rankHandler;

    public PlayerRatingService(
        IPlayerRatingRepository repository,
        IRatingTimeValidator ratingTimeValidator,
        ILogger<PlayerRatingService> logger,
        ISpecialPointsValidator specialPointsValidator,
        IRankHandler rankHandler
        )
    {
        _repository = repository;
        _ratingTimeValidator = ratingTimeValidator;
        _logger = logger;
        _specialPointsValidator = specialPointsValidator;
        _rankHandler = rankHandler;
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
        _ratingTimeValidator.EnsureRatingIsOpen();

        var ratings = await _repository.GetPlayerRatingsForPlayer(id);
        var editedRating = ratings.First(r => r.Id == id);

        var oldTotalGivenPoints = editedRating.Prediction.TotalGivenPoints;
        var oldCalculatedRank = editedRating.Prediction.CalculatedRank;

        UpdatePoints(editedRating, ratingRequestDto, ratings);
        var updatedRatings = UpdateRank(editedRating, ratings, oldTotalGivenPoints, oldCalculatedRank);

        await SaveUpdatedRatings(updatedRatings);
    }

    private ImmutableList<PlayerRating> SortRatings(ImmutableList<PlayerRating> ratings)
    {
        return ratings
            .OrderBy(r => r.Prediction.CalculatedRank ?? 100)
            .ThenBy(r => r.Country.Number)
            .ToImmutableList();
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
        _specialPointsValidator.ValidateSpecialCategoryPoints(rating, ratings);
    }

    private List<PlayerRating> UpdateRank(
        PlayerRating rating, 
        IReadOnlyList<PlayerRating> ratings, 
        int? oldTotalGivenPoints,
        int? oldCalculatedRank
        )
    {
        var updatedRatings = new List<PlayerRating>();
        if (rating.Prediction.TotalGivenPoints == oldTotalGivenPoints)
        {
            _logger.LogDebug("Skipping recalculation of rank since TotalGivenPoints is unchanged.");
            updatedRatings.Add(rating);
        }
        else
        {
            updatedRatings.Add(_rankHandler.CalculateRanks(rating, ratings, oldCalculatedRank));
        }

        return updatedRatings;
    }

    private async Task SaveUpdatedRatings(IReadOnlyList<PlayerRating> ratings)
    {
        foreach (var rating in ratings)
        {
            _logger.LogDebug("Updating rating with id={id}.", rating.Id);
            await _repository.UpdateRating(rating);
        }
    }
}