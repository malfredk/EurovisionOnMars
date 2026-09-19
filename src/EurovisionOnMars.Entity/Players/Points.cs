namespace EurovisionOnMars.Entity.Players;

public sealed record Points
{
    private static readonly HashSet<int> ValidValues =
    [
        1, 2, 3, 4, 5, 6, 7, 8, 10, 12
    ];

    public static readonly HashSet<int> SpecialPoints = [ 10, 12 ];

    public int Value { get; }

    public Points(int value)
    {
        if (!ValidValues.Contains(value))
            throw new ArgumentException($"Invalid points amount: {value}.");

        Value = value;
    }

    public bool IsSpecial => SpecialPoints.Contains(Value);
}
