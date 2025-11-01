using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultMapper
{
    public RatingGameResultDto ToDto(RatingGameResult entity);
}
public class RatingGameResultMapper : IRatingGameResultMapper
{
    public RatingGameResultDto ToDto(RatingGameResult entity)
    {
        return new RatingGameResultDto
        {
            Id = entity.Id,
            RankDifference = entity.RankDifference,
            BonusPoints = entity.BonusPoints
        };
    }
}