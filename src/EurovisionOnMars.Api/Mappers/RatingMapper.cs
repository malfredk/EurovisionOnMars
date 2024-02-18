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
        entity.Category1Points = dto.Category1;
        entity.Category2Points = dto.Category2;
        entity.Category3Points = dto.Category3;
        return entity;
    }

    public RatingDto ToDto(Rating entity)
    {
        return new RatingDto
            (
                entity.Id,
                entity.Category1Points,
                entity.Category2Points,
                entity.Category3Points,
                entity.PlayerId
            );
    }
}
