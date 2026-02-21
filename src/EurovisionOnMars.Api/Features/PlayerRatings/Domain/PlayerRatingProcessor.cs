using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings.Domain;

public interface IPlayerRatingProcessor
{
    public void UpdatePlayerRating(
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

    public void UpdatePlayerRating(
        UpdatePlayerRatingRequestDto ratingRequestDto,
        PlayerRating ratingToUpdate,
        IReadOnlyList<PlayerRating> ratings
    )
    {
        var oldTotalPoints = ratingToUpdate.Prediction.TotalGivenPoints;

        UpdatePoints(ratingToUpdate, ratingRequestDto, ratings);
        CalculatePredictions(ratingToUpdate, ratings, oldTotalPoints);
    }

    private void UpdatePoints(
        PlayerRating ratingToUpdate,
        UpdatePlayerRatingRequestDto ratingRequest,
        IReadOnlyList<PlayerRating> ratings
        )
    {
        ratingToUpdate.SetPoints(
            ratingRequest.Category1Points,
            ratingRequest.Category2Points,
            ratingRequest.Category3Points
            );
        _specialPointsValidator.ValidateSpecialCategoryPoints(ratingToUpdate, ratings);
    }

    private void CalculatePredictions(
        PlayerRating ratingWithUpdatedPoints,
        IReadOnlyList<PlayerRating> ratings,
        int? oldTotalGivenPoints
        )
    {
        if (ratingWithUpdatedPoints.Prediction.TotalGivenPoints == oldTotalGivenPoints)
        {
            _logger.LogDebug("Skipping calculation of prediction since TotalGivenPoints is unchanged.");
        }
        else
        {
            var ratingsWithCalculatedRank = _rankHandler.CalculateRanks(ratings);
            _tieBreakDemotionHandler.CalculateTieBreakDemotions(ratingWithUpdatedPoints.Prediction, ratingsWithCalculatedRank, oldTotalGivenPoints);
        }
    }
}
