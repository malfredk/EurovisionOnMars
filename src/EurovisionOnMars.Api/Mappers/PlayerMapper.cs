using EurovisionOnMars.Dto;
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
    private readonly IPlayerResultMapper _playerResultMapper;
    private readonly Utils _utils = new Utils();

    public PlayerMapper(IRatingMapper ratingMapper, IPlayerResultMapper playerResultMapper)
    {
        _ratingMapper = ratingMapper;
        _playerResultMapper = playerResultMapper;
    }

    public Player UpdateEntity(Player entity, PlayerDto dto) 
    {
        var ratingEntities = _utils.UpdateList(entity.Ratings, dto.Ratings, _ratingMapper.UpdateEntity);
        entity.Ratings = ratingEntities; // TODO: necessary to update ratings?
        return entity;
    }

    public PlayerDto ToDto(Player entity)
    {
        return new PlayerDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Ratings = Utils.MapList(entity.Ratings, _ratingMapper.ToDto),
            PlayerResult = _playerResultMapper.ToDto(entity.PlayerResult)
        };
    }
}