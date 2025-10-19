using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Services;

public interface IRatingResultService
{
    Task CalculateRatingResults(int playerId);
}

public class RatingResultService : IRatingResultService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IRatingResultRepository _ratingResultRepository;
    private readonly ILogger<RatingResultService> _logger;

    public RatingResultService
        (
        IRatingRepository ratingRepository, 
        IRatingResultRepository ratingResultRepository,
        ILogger<RatingResultService> logger
        )
    {
        _ratingRepository = ratingRepository;
        _ratingResultRepository = ratingResultRepository;
        _logger = logger;
    }

    public async Task CalculateRatingResults(int playerId)
    {
        var ratings = await _ratingRepository.GetRatingsByPlayer(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"Player with id={playerId} is missing ratings");
        }

        int rankDifference;
        int bonusPoints;
        foreach (var rating in ratings)
        {
            rankDifference = CalculateRankDifference(rating);
            bonusPoints = CalculateBonusPoints(rating, ratings, rankDifference);
            await UpdateRatingResult(rating, rankDifference, bonusPoints);
        }
    }

    private async Task UpdateRatingResult(PlayerRating rating, int? rankDifference, int? bonusPoints)
    {
        if (rankDifference == null)
        {
            return;
        }

        var ratingResult = rating.RatingGameResult;
        ratingResult.RankDifference = rankDifference;
        ratingResult.BonusPoints = bonusPoints;

        await _ratingResultRepository.UpdateRatingResult(ratingResult);
        return;
    }

    private int CalculateRankDifference(PlayerRating rating)
    {
        var actualRank = rating.Country.ActualRank;
        var expectedRank = rating.Prediction.CalculatedRank;

        if (actualRank == null)
        {
            throw new Exception("Country is missing rank");
        }
        // player is penalized for not rating a country
        else if (expectedRank == null)
        {
            return 26;
        }
        else
        {
            return (int)(actualRank - expectedRank);
        }
    }

    private int CalculateBonusPoints(PlayerRating rating, ImmutableList<PlayerRating> ratings, int rankDifference)
    {
        if (rankDifference == 0 && HasUniqueRank(rating, ratings))
        {
            return DetermineBonusPoints((int)rating.Prediction.CalculatedRank!);
        }

        return 0;
    }

    private bool HasUniqueRank(PlayerRating rating, ImmutableList<PlayerRating> ratings)
    {
        var sameRankList = ratings.Where(r => r.Prediction.CalculatedRank == rating.Prediction.CalculatedRank);
        if (sameRankList.Count() == 1)
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