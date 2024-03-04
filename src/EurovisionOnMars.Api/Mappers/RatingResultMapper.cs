using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IRatingResultMapper
{
    public RatingResultDto ToDto(RatingResult entity);
}
public class RatingResultMapper : IRatingResultMapper
{
    public RatingResultDto ToDto(RatingResult entity)
    {
        return new RatingResultDto
        {
            RankingDifference = entity.RankingDifference,
            BonusPoints = entity.BonusPoints
        };
    }
}