using EurovisionOnMars.Dto.RatingGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultMapper
{
    public RatingGameResultResponseDto ToDto(RatingGameResult entity);
}
public class RatingGameResultMapper : IRatingGameResultMapper
{
    public RatingGameResultResponseDto ToDto(RatingGameResult entity)
    {
        return new RatingGameResultResponseDto
        {
            Id = entity.Id,
            RankDifference = entity.RankDifference,
            BonusPoints = entity.BonusPoints
        };
    }
}