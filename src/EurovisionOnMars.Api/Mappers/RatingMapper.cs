using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IRatingMapper
{
    public Rating UpdateEntity(Rating entity, RatingDto dto);
    public RatingDto ToDto(Rating entity);
}

public class RatingMapper : IRatingMapper
{
    public Rating UpdateEntity(Rating entity, RatingDto dto)
    {
        entity.Category1Points = dto.Category1Points;
        entity.Category2Points = dto.Category2Points;
        entity.Category3Points = dto.Category3Points;
        entity.PointsSum = dto.Category1Points + dto.Category2Points + dto.Category3Points; // TODO: consider moving logic to service
        entity.Ranking = dto.Ranking;
        return entity;
    }

    public RatingDto ToDto(Rating entity)
    {
        return new RatingDto
        {
            Id = entity.Id,
            Category1Points = entity.Category1Points,
            Category2Points = entity.Category2Points,
            Category3Points = entity.Category3Points,
            PlayerId = entity.PlayerId,
            PointsSum = entity.PointsSum,
            Ranking = entity.Ranking
        };
    }
}