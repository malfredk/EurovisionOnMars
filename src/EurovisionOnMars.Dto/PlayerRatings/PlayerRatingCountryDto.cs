namespace EurovisionOnMars.Dto.PlayerRatings;

public record PlayerRatingCountryDto
{
    public int Number { get; set; }
    public required string Name { get; set; }
}
