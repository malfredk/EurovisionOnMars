using EurovisionOnMars.Entity.Countries;

namespace EurovisionOnMars.Entity.Players.PlayerRatings;

public sealed record BonusPoints
{
    private static readonly IReadOnlyDictionary<int, int> BonusByRank =
        new Dictionary<int, int>
        {
            [1] = -25,
            [2] = -18,
            [3] = -15,
            [4] = -12,
            [5] = -10,
            [6] = -8,
            [7] = -6,
            [8] = -4,
            [9] = -2,
            [10] = -1
        };

    public int Value { get; }

    public BonusPoints(int value)
    {
        if (value != 0 && !BonusByRank.Values.Contains(value))
        {
            throw new ArgumentException(
                $"Invalid bonus points amount: {value}.");
        }

        Value = value;
    }

    public static BonusPoints FromRank(CountryPosition rank)
    {
        var value = BonusByRank.GetValueOrDefault(rank.Value, 0);

        return new BonusPoints(value);
    }
}