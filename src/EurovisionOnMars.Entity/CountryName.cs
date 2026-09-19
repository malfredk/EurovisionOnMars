using System.Collections.Immutable;

namespace EurovisionOnMars.Entity;

public sealed record CountryName
{
    public string Value { get; }

    public CountryName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = value.Trim().ToLowerInvariant();

        if (!ValidNames.Contains(normalized))
            throw new ArgumentException(
                $"'{value}' is not a valid country name.",
                nameof(value));

        Value = normalized;
    }

    private static readonly ImmutableHashSet<string> ValidNames = [
            "australia",
            "tsjekkia",
            "armenia",
            "serbia",
            "moldova",
            "ukraina",
            "albania",
            "litauen",
            "polen",
            "kroatia",
            "estland",
            "slovenia",
            "kypros",
            "israel",
            "italia",
            "portugal",
            "østerrike",
            "finland",
            "norge",
            "spania",
            "sverige",
            "sveits",
            "belgia",
            "frankrike",
            "storbritannia",
            "tyskland",
            "aserbajdsjan",
            "romania",
            "island",
            "hellas",
            "nederland",
            "san marino",
            "bulgaria",
            "russland",
            "malta",
            "belarus",
            "nord-makedonia",
            "danmark",
            "ungarn",
            "irland",
            "georgia",
            "latvia",
            "montenegro",
            "bosnia-hercegovina",
            "tyrkia",
            "slovakia",
            "luxembourg",
            "monaco"
            ];

    public override string ToString() => Value;
}
