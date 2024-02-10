using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public class PlayerMapper
{
    private readonly RatingMapper _ratingMapper = new RatingMapper();

    public Player UpdateEntity(Player entity, PlayerDto dto) 
    {
        // TODO: update fields that are editeble
        return entity;
    }

    public PlayerDto ToDto(Player entity)
    {
        List<RatingDto> ratingDtos = null;
        if (entity.Ratings != null)
        {
            ratingDtos = entity.Ratings.Select(rating => _ratingMapper.ToDto(rating)).ToList();
        }
        
        return new PlayerDto
            (
                entity.Id, 
                entity.Username,
                ratingDtos
            );
    }
}
