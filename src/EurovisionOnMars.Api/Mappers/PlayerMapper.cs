using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IPlayerMapper
{
    public PlayerDto ToDto(Player entity);
}

public class PlayerMapper : IPlayerMapper
{
    private readonly IRatingMapper _ratingMapper;
    private readonly IPlayerResultMapper _playerResultMapper;

    public PlayerMapper(IRatingMapper ratingMapper, IPlayerResultMapper playerResultMapper)
    {
        _ratingMapper = ratingMapper;
        _playerResultMapper = playerResultMapper;
    }

    public PlayerDto ToDto(Player entity)
    {
        var playerResult = entity.PlayerResult is null ? 
            null : _playerResultMapper.ToDto(entity.PlayerResult!);
        return new PlayerDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Ratings = Utils.MapList(entity.Ratings, _ratingMapper.ToDto),
            PlayerResult = playerResult
        };
    }
}