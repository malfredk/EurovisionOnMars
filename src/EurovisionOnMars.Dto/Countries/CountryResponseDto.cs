namespace EurovisionOnMars.Dto.Countries;

public record CountryResponseDto : IdBaseDto
{
    public required int Number { get; set; }
    public required string Name { get; set; }
    public int? ActualRank { get; set; }
}

