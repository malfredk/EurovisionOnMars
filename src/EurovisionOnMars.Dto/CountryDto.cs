namespace EurovisionOnMars.Dto;

public record CountryDto : IdBaseDto
{
    public required int Number { get; set; }
    public required string Name { get; set; }
    public int? Ranking { get; set; }
}
