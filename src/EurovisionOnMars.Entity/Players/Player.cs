using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Players;

public class Player : IdBase
{
    public Username Username { get; private set; } = null!;
    public List<PlayerRating> PlayerRatings { get; private set; } = [];
    public PlayerGameResult PlayerGameResult { get; private set; } = null!;

    private Player() { }

    public Player(Username username, ImmutableList<Country> countries)
    {
        ValidateCountries(countries);
        Username = username;

        PlayerRatings = countries
            .Select(c => new PlayerRating(this, c))
            .ToList();

        PlayerGameResult = new PlayerGameResult(this);
    }

    private void ValidateCountries(ImmutableList<Country> countries)
    {
        if (countries.IsNullOrEmpty())
        {
            throw new InvalidOperationException("Country list is empty; therefore user cannot be created.");
        }
    }

    public void CalculateGamePoints()
    {
        CalculateRatingGameResults();
        CalculateTotalPoints();
    }

    private void CalculateRatingGameResults()
    {
        CalculateRankDifferences();
        CalculateBonusPoints();
    }

    private void CalculateRankDifferences()
    {
        foreach (var rating in PlayerRatings)
        {
            rating.CalculateRankDifference();
        }
    }

    private void CalculateBonusPoints()
    {
        var uniquePredictedRanks = GetUniquePredictedRanks();

        foreach (var rating in PlayerRatings)
        {
            var predictedRank = rating.Prediction.GetPredictedRank();

            var hasUniquePredictedRank =
                predictedRank != null &&
                uniquePredictedRanks.Contains(predictedRank);

            rating.CalculateBonusPoints(hasUniquePredictedRank);
        }
    }

    private HashSet<CountryPosition> GetUniquePredictedRanks()
    {
        return PlayerRatings
            .Select(r => r.Prediction.GetPredictedRank())
            .Where(rank => rank != null)
            .GroupBy(rank => rank!)
            .Where(group => group.Count() == 1)
            .Select(group => group.Key)
            .ToHashSet();
    }

    private void CalculateTotalPoints()
    {
        var totalPoints = PlayerRatings.Sum(rating =>
        {
            var result = rating.RatingGameResult;

            var bonusPoints = result.BonusPoints
                ?? throw new InvalidOperationException(
                    "Bonus points are missing.");

            var rankDifference = result.RankDifference
                ?? throw new InvalidOperationException(
                    "Rank difference is missing.");

            return bonusPoints.Value + Math.Abs(rankDifference);
        });

        PlayerGameResult.SetTotalPoints(totalPoints);
    }
}