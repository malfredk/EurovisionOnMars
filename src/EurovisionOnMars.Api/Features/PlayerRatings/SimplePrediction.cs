namespace EurovisionOnMars.Api.Features.PlayerRatings;

public record SimplePrediction
{
    internal int? TotalGivenPoints { get; set; }
    internal int? CalculatedRank { get; set; }
}
