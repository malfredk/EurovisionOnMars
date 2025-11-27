namespace EurovisionOnMars.Dto.PlayerRatings;

public record PlayerRatingCountryResponseDto
{
    public int Number { get; set; }
    public required string Name { get; set; }
}
