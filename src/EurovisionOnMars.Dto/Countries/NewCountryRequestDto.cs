namespace EurovisionOnMars.Dto.Countries;

public record NewCountryRequestDto
{
    public required int Number { get; set; }
    public required string Name { get; set; }
}
