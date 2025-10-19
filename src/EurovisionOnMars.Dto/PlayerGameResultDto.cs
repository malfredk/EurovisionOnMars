namespace EurovisionOnMars.Dto;

public record PlayerGameResultDto : IdBaseDto
{
    public int? Rank { get; set; }
    public int? TotalPoints { get; set; }
}