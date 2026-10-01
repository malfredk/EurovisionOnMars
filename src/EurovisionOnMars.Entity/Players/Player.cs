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

    public void RateCountry(
        int ratingId,
        Points category1Points,
        Points category2Points,
        Points category3Points
        )
    {
        var rating = PlayerRatings
            .SingleOrDefault(r => r.Id == ratingId)
            ?? throw new KeyNotFoundException(
                $"No player rating with id={ratingId} exists for this player.");

        ValidateSpecialPoints(
            rating,
            category1Points,
            category2Points,
            category3Points);

        var oldTotalPoints = rating.Prediction.TotalGivenPoints;

        rating.SetPoints(
            category1Points,
            category2Points,
            category3Points);

        CalculatePredictions(rating, oldTotalPoints);
    }

    private void ValidateSpecialPoints(
        PlayerRating ratingToUpdate,
        Points category1Points,
        Points category2Points,
        Points category3Points)
    {
        Func<PlayerRating, Points?> category1PointsGetter = r => r.Category1Points;
        ValidateSpecialCategoryPoints(ratingToUpdate, category1Points, category1PointsGetter);

        Func<PlayerRating, Points?> category2PointsGetter = r => r.Category2Points;
        ValidateSpecialCategoryPoints(ratingToUpdate, category2Points, category2PointsGetter);

        Func<PlayerRating, Points?> category3PointsGetter = r => r.Category3Points;
        ValidateSpecialCategoryPoints(ratingToUpdate, category3Points, category3PointsGetter);
    }

    private void ValidateSpecialCategoryPoints(
        PlayerRating ratingToUpdate,
        Points newPoints,
        Func<PlayerRating, Points?> pointsSelector
        )
    {
        if (!newPoints.IsSpecial)
        {
            return;
        }

        EnsureUniqueSpecialPoints(ratingToUpdate, newPoints, pointsSelector);
    }

    private void EnsureUniqueSpecialPoints(
        PlayerRating ratingToUpdate,
        Points newPoints,
        Func<PlayerRating, Points?> pointsSelector
        )
    {
        var hasGivenPointsToOtherCountry = PlayerRatings
            .AsReadOnly()
            .Where(r => r.Id != ratingToUpdate.Id)
            .Any(r => pointsSelector(r) == newPoints);

        if (hasGivenPointsToOtherCountry)
        {
            throw new ArgumentException("These special points have already been given in this category to another country.");
        }
    }

    private void CalculatePredictions(
        PlayerRating ratingWithUpdatedPoints,
        int? oldTotalGivenPoints
        )
    {
        if (ratingWithUpdatedPoints.Prediction.TotalGivenPoints == oldTotalGivenPoints)
        {
            return;
        }
        else
        {
            CalculateRanks();
            CalculateTieBreakDemotions(ratingWithUpdatedPoints, oldTotalGivenPoints);
        }
    }

    void CalculateRanks()
    {
        var orderedRatings = GetSortedRatingsByDescendingPoints();

        Prediction? previousPrediction = null;
        for (int i = 0; i < orderedRatings.Count; i++)
        {
            var currentRating = orderedRatings[i];
            var currentPrediction = currentRating.Prediction;
            var currentPoints = currentPrediction.TotalGivenPoints;

            if (currentPoints == null)
            {
                break;
            }
            else if (previousPrediction != null && currentPoints == previousPrediction.TotalGivenPoints)
            {
                currentPrediction.SetCalculatedRank(previousPrediction.CalculatedRank!);
            }
            else
            {
                currentPrediction.SetCalculatedRank(new CountryPosition(i + 1));
            }
            previousPrediction = currentPrediction;
        }
    }

    private List<PlayerRating> GetSortedRatingsByDescendingPoints()
    {
        return PlayerRatings
            .OrderBy(r => r.Prediction.TotalGivenPoints == null)
            .ThenByDescending(r => r.Prediction.TotalGivenPoints)
            .ToList();
    }

    public void CalculateTieBreakDemotions(PlayerRating updatedRating, int? oldTotalPoints)
    {
        var prediction = updatedRating.Prediction;
        prediction.ResetTieBreakDemotion();

        var predictionsGroupedByPoints = PlayerRatings
            .Select(r => r.Prediction)
            .GroupBy(p => p.TotalGivenPoints)
            .ToList();

        HandleOldPointsGroup(predictionsGroupedByPoints, oldTotalPoints);
        HandleNewPointsGroup(predictionsGroupedByPoints, prediction);
    }

    private void HandleOldPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByPoints, int? oldTotalPoints)
    {
        var oldGroup = GetPredictionPointsGroup(predictionsGroupedByPoints, oldTotalPoints);

        if (oldGroup == null || oldTotalPoints == null)
        {
            // old prediction was not tied or not set; thus, there is no TieBreakDemotion to adjust
            return;
        }

        HandleTieBreakDemotions(oldGroup);
    }

    private void HandleNewPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByPoints, Prediction newPrediction)
    {
        var newGroup = GetPredictionPointsGroup(predictionsGroupedByPoints, newPrediction.TotalGivenPoints);

        HandleTieBreakDemotions(newGroup!);
    }

    private List<Prediction>? GetPredictionPointsGroup(
        List<IGrouping<int?, Prediction>> predictionsGroupedByPoints,
        int? totalGivenPoints
        )
    {
        if (totalGivenPoints == null)
        {
            return null;
        }

        var group = predictionsGroupedByPoints
            .FirstOrDefault(g => g.Key == totalGivenPoints);

        if (group == null)
        {
            return null;
        }

        return group.ToList();
    }

    private void HandleTieBreakDemotions(List<Prediction> predictionsWithSamePoints)
    {
        if (predictionsWithSamePoints.Count() == 1)
        {
            HandleSingletonList(predictionsWithSamePoints);
            return;
        }

        if (AreAllTieBreakDemotionsNull(predictionsWithSamePoints))
        {
            return;
        }

        CalculateTieBreakDemotions(predictionsWithSamePoints);
    }

    private void HandleSingletonList(List<Prediction> singlePredictionList)
    {
        var singlePrediction = singlePredictionList.First();
        singlePrediction.ResetTieBreakDemotion();
    }

    private bool AreAllTieBreakDemotionsNull(List<Prediction> predictions)
    {
        return predictions.All(p => p.TieBreakDemotion == null);
    }

    private void CalculateTieBreakDemotions(List<Prediction> predictionsWithSamePoints)
    {
        var sortedPredictions = predictionsWithSamePoints
            .OrderBy(p => p.TieBreakDemotion?.Value);

        int tieBreakDemotionValue = 0;
        foreach (var prediction in sortedPredictions)
        {
            TieBreakDemotion tieBreakDemotion = new(tieBreakDemotionValue);
            prediction.SetTieBreakDemotion(tieBreakDemotion);
            tieBreakDemotionValue++;
        }
    }
}