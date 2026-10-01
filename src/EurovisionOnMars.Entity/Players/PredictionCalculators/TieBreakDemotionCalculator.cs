using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Players.PlayerRatings.Predictions;

namespace EurovisionOnMars.Entity.Players.PredictionCalculators;

internal static class TieBreakDemotionCalculator
{
    internal static void Calculate(
       PlayerRating updatedRating,
       List<PlayerRating> ratings,
       int? oldTotalPoints
       )
    {
        var prediction = updatedRating.Prediction;
        prediction.ResetTieBreakDemotion();

        var predictionsGroupedByPoints = ratings
            .Select(r => r.Prediction)
            .GroupBy(p => p.TotalGivenPoints)
            .ToList();

        HandleOldPointsGroup(predictionsGroupedByPoints, oldTotalPoints);
        HandleNewPointsGroup(predictionsGroupedByPoints, prediction);
    }

    private static void HandleOldPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByPoints, int? oldTotalPoints)
    {
        var oldGroup = GetPredictionPointsGroup(predictionsGroupedByPoints, oldTotalPoints);

        if (oldGroup == null || oldTotalPoints == null)
        {
            // old prediction was not tied or not set; thus, there is no TieBreakDemotion to adjust
            return;
        }

        HandleTieBreakDemotions(oldGroup);
    }

    private static void HandleNewPointsGroup(List<IGrouping<int?, Prediction>> predictionsGroupedByPoints, Prediction newPrediction)
    {
        var newGroup = GetPredictionPointsGroup(predictionsGroupedByPoints, newPrediction.TotalGivenPoints);

        HandleTieBreakDemotions(newGroup!);
    }

    private static List<Prediction>? GetPredictionPointsGroup(
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

    private static void HandleTieBreakDemotions(List<Prediction> predictionsWithSamePoints)
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

    private static void HandleSingletonList(List<Prediction> singlePredictionList)
    {
        var singlePrediction = singlePredictionList.First();
        singlePrediction.ResetTieBreakDemotion();
    }

    private static bool AreAllTieBreakDemotionsNull(List<Prediction> predictions)
    {
        return predictions.All(p => p.TieBreakDemotion == null);
    }

    private static void CalculateTieBreakDemotions(List<Prediction> predictionsWithSamePoints)
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
