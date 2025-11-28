namespace EurovisionOnMars.Dto.PlayerRatings;

public record UpdatePlayerRatingRequestDto
{
    public int Category1Points { get; set; }
    public int Category2Points { get; set; }
    public int Category3Points { get; set;}
}