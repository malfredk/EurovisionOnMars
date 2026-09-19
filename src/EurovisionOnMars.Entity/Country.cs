using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public class Country : IdBase
{
    public CountryPosition Number { get; private set; }
    public CountryName Name { get; private set; }
    public CountryPosition? ActualRank { get; private set; }
    [JsonIgnore]
    public List<PlayerRating>? PlayerRatings { get; }

    private Country() { }

    public Country(CountryPosition number, CountryName name)
    {
        Number = number;
        Name = name;
    }

    public void SetActualRank(CountryPosition rank)
    {
        ActualRank = rank;
    }
}