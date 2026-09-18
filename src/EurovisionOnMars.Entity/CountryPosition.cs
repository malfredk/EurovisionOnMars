namespace EurovisionOnMars.Entity;

public sealed record CountryPosition
{
    private const int MinValue = 1;
    private const int MaxValue = 26;

    public int Value { get; }

    private CountryPosition(int value)
    {
        Value = value;
    }

    public static CountryPosition Create(int value)
    {
        if (value < MinValue || value > MaxValue)
            throw new ArgumentException($"Invalid country position: {value}.");

        return new CountryPosition(value);
    }
}
