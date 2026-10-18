namespace EurovisionOnMars.Dto.Players.ResolveTieBreak;

public record ResolveTieBreakRequestDto
{
    public List<int> OrderedPredictionIds { get; set; } = new();
}
