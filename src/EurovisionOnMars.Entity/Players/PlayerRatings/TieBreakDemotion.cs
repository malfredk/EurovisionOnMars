namespace EurovisionOnMars.Entity.Players.PlayerRatings;

public sealed record TieBreakDemotion
{
    public int Value { get; }

    public TieBreakDemotion(int value)
    {
        if (value < 0)
            throw new ArgumentException($"Invalid tie break demotion: {value}.");

        Value = value;
    }
}
