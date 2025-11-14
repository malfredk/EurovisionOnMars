using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IRankHandler
{
    public IReadOnlyList<PlayerRating> CalculateRanks(IReadOnlyList<PlayerRating> ratings);
}

public class RankHandler : IRankHandler
{
    public IReadOnlyList<PlayerRating> CalculateRanks(IReadOnlyList<PlayerRating> ratings) // TODO: test
    {
        var orderedPredicitions = ratings
            .Select(r => r.Prediction)
            .OrderByDescending(p => p.TotalGivenPoints)
            .ToList();

        Prediction? previous = null;
        for (int i = 0; i < orderedPredicitions.Count; i++)
        {
            var current = orderedPredicitions[i];
            if (previous != null && current.TotalGivenPoints == previous.TotalGivenPoints)
            {
                current.SetCalculatedRank(previous.CalculatedRank);
            }
            else
            {
                current.SetCalculatedRank(i + 1);
            }
            previous = current;
        }

        return ratings;
    }
}
