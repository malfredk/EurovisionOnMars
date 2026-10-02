namespace EurovisionOnMars.Dto.Players.RateCountry;

public record RateCountryRequestDto
{
    public int Category1Points { get; set; }
    public int Category2Points { get; set; }
    public int Category3Points { get; set;}
}