namespace EurovisionOnMars.Dto.Predictions;

public record ResolveTieBreakRequestDto
{
    public List<int> OrderedPredictionIds { get; set; } = new();
}
