using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public class PlayerMapper
{
    public Player UpdateEntity(Player entity, PlayerDto dto) 
    {
        // TODO: update fields that are editeble
        return entity;
    }

    public PlayerDto ToDto(Player entity)
    {
        return new PlayerDto
            (
                entity.Id, 
                entity.Username
            );
    }
}
