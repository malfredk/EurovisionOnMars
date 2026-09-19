using System.Text.RegularExpressions;

namespace EurovisionOnMars.Entity;

public sealed record Username
{
    private const int MaxLength = 12;
    private static readonly Regex Pattern =
        new(@"^[a-zA-Z0-9æøåÆØÅ]+$", RegexOptions.Compiled);

    public string Value { get; }

    public Username(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Username cannot be longer than {MaxLength} characters.",
                nameof(value));
        }

        if (!Pattern.IsMatch(value))
        {
            throw new ArgumentException(
                "Username contains invalid characters.",
                nameof(value));
        }

        Value = value;
    }

    public override string ToString() => Value;
}