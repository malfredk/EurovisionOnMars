namespace EurovisionOnMars.Dto.GameResults.GetPlayerResults;

public record PlayerGameResultDto
{
    public int? Rank { get; set; }
    public int? TotalPoints { get; set; }
    public required string PlayerUsername { get; set; }
}