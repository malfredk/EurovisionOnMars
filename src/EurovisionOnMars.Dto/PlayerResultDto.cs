namespace EurovisionOnMars.Dto;

public record PlayerResultDto : IdBaseDto
{
    public int? Ranking { get; set; }
    public int? Score { get; set; }
}