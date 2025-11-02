using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultService
{
    Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId);
    Task CalculateRatingGameResults();
}

public class RatingGameResultService : IRatingGameResultService
{
    private readonly IRatingGameResultRepository _ratingGameResultRepository;
    private readonly IPlayerRatingService _playerRatingService;
    private readonly ILogger<RatingGameResultService> _logger;

    public RatingGameResultService
        (
        IRatingGameResultRepository ratingResultRepository,
        IPlayerRatingService playerRatingService,
        ILogger<RatingGameResultService> logger
        )
    {
        _ratingGameResultRepository = ratingResultRepository;
        _playerRatingService = playerRatingService;
        _logger = logger;
    }

    public async Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId)
    {
        return await _ratingGameResultRepository.GetRatingGameResults(playerId);
    }

    public async Task CalculateRatingGameResults()
    {
        var playerRatings = await _playerRatingService.GetAllPlayerRatings();
        foreach (var playerRating in playerRatings)
        {
            var ratingGameResult = await CalculateRatingGameResult(playerRating, playerRatings);
            await UpdateRatingGameResult(ratingGameResult);
        }
    }

    public async Task<RatingGameResult> CalculateRatingGameResult(PlayerRating rating, IReadOnlyList<PlayerRating> ratings)
    {
        int rankDifference = CalculateRankDifference(rating);
        int bonusPoints = CalculateBonusPoints(rating, ratings, rankDifference);

        var ratingGameResult = rating.RatingGameResult;
        ratingGameResult.RankDifference = rankDifference;
        ratingGameResult.BonusPoints = bonusPoints;
        return ratingGameResult;
    }

    private async Task UpdateRatingGameResult(RatingGameResult ratingGameResult)
    {
         await _ratingGameResultRepository.UpdateRatingGameResult(ratingGameResult);
    }

    private int CalculateRankDifference(PlayerRating rating)
    {
        var actualRank = rating.Country.ActualRank;
        var predictedRank = rating.Prediction.CalculatedRank;

        if (actualRank == null)
        {
            throw new Exception("Country is missing rank");
        }
        else if (predictedRank == null)
        {
            // player is penalized for not rating a country
            return 26;
        }
        else
        {
            return (int)(actualRank - predictedRank);
        }
    }

    private int CalculateBonusPoints(
        PlayerRating rating,
        IReadOnlyList<PlayerRating> ratings,
        int rankDifference
    )
    {
        if (rankDifference == 0 && HasUniqueRank(rating, ratings))
        {
            return DetermineBonusPoints((int)rating.Prediction.CalculatedRank);
        }
        return 0;
    }

    private bool HasUniqueRank(PlayerRating rating, IReadOnlyList<PlayerRating> ratings)
    {
        var sameRankAndPlayerList = ratings
            .Where(r => 
            r.PlayerId == rating.PlayerId && 
            r.Prediction.CalculatedRank == rating.Prediction.CalculatedRank
            );
        if (sameRankAndPlayerList.Count() == 1)
        {
            return true;
        }
        return false;
    }

    private int DetermineBonusPoints(int rank)
    {
        switch (rank)
        {
            case 1:
                return -25;
            case 2:
                return -18;
            case 3:
                return -15;
            case 4:
                return -12;
            case 5:
                return -10;
            case 6:
                return -8;
            case 7:
                return -6;
            case 8:
                return -4;
            case 9:
                return -2;
            case 10:
                return -1;
            default:
                return 0;
        }
    }
}