namespace EurovisionOnMars.Entity.Players;

public sealed record PlayerRank
{
    public int Value { get; }

    public PlayerRank(int value)
    {
        if (value < 1)
            throw new ArgumentException($"Invalid rank for player: {value}.");

        Value = value;
    }
}
