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

        int rankingDifference;
        int bonusPoints;
        foreach (var rating in ratings)
        {
            rankingDifference = CalculateRankingDifference(rating);
            bonusPoints = CalculateBonusPoints(rating, ratings, rankingDifference);
            await UpdateRatingResult(rating, rankingDifference, bonusPoints);
        }
    }

    private async Task UpdateRatingResult(Rating rating, int? rankingDifference, int? bonusPoints)
    {
        if (rankingDifference == null)
        {
            return;
        }

        var ratingResult = rating.RatingResult;
        ratingResult.RankingDifference = rankingDifference;
        ratingResult.BonusPoints = bonusPoints;

        await _ratingResultRepository.UpdateRatingResult(ratingResult);
        return;
    }

    private int CalculateRankingDifference(Rating rating)
    {
        var actualRanking = rating.Country.Ranking;
        var expectedRanking = rating.Ranking;

        if (actualRanking == null)
        {
            throw new Exception("Country is missing ranking");
        }
        // player is penalized for not rating a country
        else if (expectedRanking == null)
        {
            return 26;
        }
        else
        {
            return (int)(actualRanking - expectedRanking);
        }
    }

    private int CalculateBonusPoints(Rating rating, ImmutableList<Rating> ratings, int rankingDifference)
    {
        if (rankingDifference == 0 && HasUniqueRanking(rating, ratings))
        {
            return DetermineBonusPoints((int)rating.Ranking!);
        }

        return 0;
    }

    private bool HasUniqueRanking(Rating rating, ImmutableList<Rating> ratings)
    {
        var sameRankingList = ratings.Where(r => r.Ranking == rating.Ranking);
        if (sameRankingList.Count() == 1)
        {
            return true;
        }
        return false;
    }

    private int DetermineBonusPoints(int ranking)
    {
        switch (ranking)
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