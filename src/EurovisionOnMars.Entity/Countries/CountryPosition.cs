namespace EurovisionOnMars.Entity.Countries;

public sealed record CountryPosition
{
    private const int MinValue = 1;
    private const int MaxValue = 26;

    public int Value { get; }

    public CountryPosition(int value)
    {
        if (value < MinValue || value > MaxValue)
            throw new ArgumentException($"Invalid country position: {value}.");

        Value = value;
    }
}
