using EurovisionOnMars.Domain.Countries;

namespace EurovisionOnMars.Domain.Players.Predictions;

internal static class TieBreakResolver
{
    internal static void Resolve(List<Prediction> predictions, List<int> orderedPredictionIds)
    {
        ValidateIdList(predictions, orderedPredictionIds);
        SetTieBreakDemotions(predictions, orderedPredictionIds);
    }

    private static void ValidateIdList(List<Prediction> predictions, List<int> orderedPredictionIds)
    {
        EnsureMultiplePredictions(orderedPredictionIds);
        EnsureUniquePredictions(orderedPredictionIds);
        EnsureMatchingTiedPredictions(predictions, orderedPredictionIds);
    }

    private static void EnsureMultiplePredictions(List<int> orderedPredictionIds)
    {
        if (orderedPredictionIds.Count <= 1)
        {
            throw new ArgumentException("There is no tie break to resolve since orderedPredictionIds does not contain multiple ids.");
        }
    }

    private static void EnsureUniquePredictions(List<int> orderedPredictionIds)
    {
        if (orderedPredictionIds.Distinct().Count() != orderedPredictionIds.Count)
        {
            throw new ArgumentException("The list, orderedPredictionIds, contains duplicate ids.");
        }
    }

    private static void EnsureMatchingTiedPredictions(List<Prediction> predictions, List<int> orderedPredictionIds)
    {
        var calculatedRank = GetCalculatedRank(predictions, orderedPredictionIds[0]);

        var tiedPredictionIds = GetTiedPredictionIds(predictions, calculatedRank);
        var requestIds = orderedPredictionIds.ToHashSet();

        if (!tiedPredictionIds.SetEquals(requestIds))
        {
            throw new ArgumentException("One or more prediction ids in request do not match tied predictions in database.");
        }
    }

    private static CountryPosition GetCalculatedRank(List<Prediction> predictions, int predicitonId)
    {
        return predictions
            .Single(p => p.Id == predicitonId)
            .CalculatedRank ?? throw new ArgumentException($"Prediction with id={predicitonId} does not have a CalculatedRank.");
    }

    private static HashSet<int> GetTiedPredictionIds(List<Prediction> predictions, CountryPosition calculatedRank)
    {
        return predictions
            .Where(p => p.CalculatedRank == calculatedRank)
            .Select(p => p.Id)
            .ToHashSet();
    }

    private static void SetTieBreakDemotions(List<Prediction> predictions, List<int> orderedPredictionIds)
    {
        var predictionsById = predictions.ToDictionary(p => p.Id);
        for (int i = 0; i < orderedPredictionIds.Count; i++)
        {
            var id = orderedPredictionIds[i];
            var tieBreakDemotion = new TieBreakDemotion(i);

            var prediction = predictionsById[id];
            prediction.SetTieBreakDemotion(tieBreakDemotion);
        }
    }
}
