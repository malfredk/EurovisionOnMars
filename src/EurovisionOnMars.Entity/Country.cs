using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public class Country : IdBase
{
    private static ImmutableList<string> POSSIBLE_PARTICIPANTS =
    [
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

    public CountryPosition Number { get; private set; }
    public string Name { get; private set; } = null!;
    public CountryPosition? ActualRank { get; private set; }
    [JsonIgnore]
    public List<PlayerRating>? PlayerRatings { get; }

    private Country() { }

    public Country(CountryPosition number, string name)
    {
        ValidateName(name);

        Number = number;
        Name = name;
    }

    public void SetActualRank(CountryPosition rank)
    {
        ActualRank = rank;
    }

    private void ValidateName(string name)
    {
        var isValid = POSSIBLE_PARTICIPANTS.Contains(name);
        if (!isValid)
        {
            throw new ArgumentException("Invalid name of country");
        }
    }
}