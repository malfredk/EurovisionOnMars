using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings.Domain;

public interface IPlayerRatingProcessor
{
    public void UpdatePlayerRatings(
        UpdatePlayerRatingRequestDto ratingRequestDto,
        PlayerRating editedRating,
        IReadOnlyList<PlayerRating> ratings
    );
}

public class PlayerRatingProcessor : IPlayerRatingProcessor
{
    private readonly ILogger<PlayerRatingProcessor> _logger;
    private readonly ISpecialPointsValidator _specialPointsValidator;
    private readonly IRankHandler _rankHandler;
    private readonly ITieBreakDemotionHandler _tieBreakDemotionHandler;

    public PlayerRatingProcessor(
        ILogger<PlayerRatingProcessor> logger,
        ISpecialPointsValidator specialPointsValidator,
        IRankHandler rankHandler,
        ITieBreakDemotionHandler tieBreakDemotionHandler
        )
    {
        _logger = logger;
        _specialPointsValidator = specialPointsValidator;
        _rankHandler = rankHandler;
        _tieBreakDemotionHandler = tieBreakDemotionHandler;
    }

    public void UpdatePlayerRatings(
        UpdatePlayerRatingRequestDto ratingRequestDto,
        PlayerRating editedRating,
        IReadOnlyList<PlayerRating> ratings
    )
    {
        var oldTotalPoints = editedRating.Prediction.TotalGivenPoints;

        UpdatePoints(editedRating, ratingRequestDto, ratings);
        CalculatePredictions(editedRating, ratings, oldTotalPoints);
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

    private void CalculatePredictions(
        PlayerRating editedRating,
        IReadOnlyList<PlayerRating> ratings,
        int? oldTotalGivenPoints
        )
    {
        if (editedRating.Prediction.TotalGivenPoints == oldTotalGivenPoints)
        {
            _logger.LogDebug("Skipping calculation of prediction since TotalGivenPoints is unchanged.");
        }
        else
        {
            var ratingsWithCalculatedRank = _rankHandler.CalculateRanks(ratings);
            _tieBreakDemotionHandler.CalculateTieBreakDemotions(editedRating.Prediction, ratingsWithCalculatedRank, oldTotalGivenPoints);
        }
    }
}
