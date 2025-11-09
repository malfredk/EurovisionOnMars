using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record Country : IdBase
{
    private static int MIN_NUMBER = 1;
    private static int MAX_NUMBER = 26;
    private static ImmutableList<string> POSSIBLE_PARTICIPANTS = new List<string>
    {
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
    }.ToImmutableList();

    public int Number { get; private set; }
    public string Name { get; private set; } = null!;
    public int? ActualRank { get; private set; }
    [JsonIgnore]
    public List<PlayerRating>? PlayerRatings { get; }

    private Country() { }

    public Country(int number, string name)
    {
        ValidateNumber(number);
        ValidateName(name);

        Number = number;
        Name = name;
    }

    public void SetActualRank(int rank)
    {
        ValidateNumber(rank);
        ActualRank = rank;
    }

    private void ValidateNumber(int? number)
    {
        var isValid = number != null
            && number >= MIN_NUMBER
            && number <= MAX_NUMBER;

        if (!isValid)
        {
            throw new ArgumentException("Invalid number or rank for country");
        }
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