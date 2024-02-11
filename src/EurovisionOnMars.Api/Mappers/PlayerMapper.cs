﻿using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IPlayerMapper
{
    public Player UpdateEntity(Player entity, PlayerDto dto);
    public PlayerDto ToDto(Player entity);
}

public class PlayerMapper : IPlayerMapper
{
    private readonly IRatingMapper _ratingMapper;
    private readonly Utils _utils = new Utils();

    public PlayerMapper(IRatingMapper ratingMapper)
    {
        _ratingMapper = ratingMapper;
    }

    public Player UpdateEntity(Player entity, PlayerDto dto) 
    {
        var ratingEntities = _utils.UpdateList(entity.Ratings, dto.Ratings, _ratingMapper.UpdateEntity);
        entity.Ratings = ratingEntities;
        return entity;
    }

    public PlayerDto ToDto(Player entity)
    {
        return new PlayerDto
            (
                entity.Id, 
                entity.Username,
                Utils.MapList(entity.Ratings, _ratingMapper.ToDto)
            );
    }
}
