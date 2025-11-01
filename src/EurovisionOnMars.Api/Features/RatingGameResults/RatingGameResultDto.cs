using EurovisionOnMars.Api.Features.Common;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public record RatingGameResultDto : IdBaseDto
{
    public int? RankDifference { get; set; }
    public int? BonusPoints { get; set; }
}