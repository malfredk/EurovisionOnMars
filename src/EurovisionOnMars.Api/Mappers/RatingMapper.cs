using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public class RatingMapper
{
    public Rating UpdateEntity(Rating entity, RatingDto dto)
    {
        entity.Category1 = dto.Category1;
        entity.Category2 = dto.Category2;
        entity.Category3 = dto.Category3;
        return entity;
    }

    public RatingDto ToDto(Rating entity)
    {
        return new RatingDto
            (
                entity.Id,
                entity.Category1,
                entity.Category2,
                entity.Category3,
                entity.PlayerId
            );
    }
}
