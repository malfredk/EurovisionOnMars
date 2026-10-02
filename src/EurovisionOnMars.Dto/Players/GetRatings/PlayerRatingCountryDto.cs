namespace EurovisionOnMars.Dto.Players.GetRatings;

public record PlayerRatingCountryDto
{
    public int Number { get; set; }
    public required string Name { get; set; }
}
