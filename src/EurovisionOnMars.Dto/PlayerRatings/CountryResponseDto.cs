namespace EurovisionOnMars.Dto.PlayerRatings;

public record CountryResponseDto
{
    public int Number { get; set; }
    public required string Name { get; set; }
}
