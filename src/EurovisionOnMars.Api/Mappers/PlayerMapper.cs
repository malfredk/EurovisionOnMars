using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IPlayerMapper
{
    public PlayerDto ToDto(Player entity);
}

public class PlayerMapper : IPlayerMapper
{
    private readonly IPlayerRatingMapper _ratingMapper;
    private readonly IPlayerGameResultMapper _playerResultMapper;

    public PlayerMapper(IPlayerRatingMapper ratingMapper, IPlayerGameResultMapper playerResultMapper)
    {
        _ratingMapper = ratingMapper;
        _playerResultMapper = playerResultMapper;
    }

    public PlayerDto ToDto(Player entity)
    {
        var playerResult = entity.PlayerGameResult is null ? 
            null : _playerResultMapper.ToDto(entity.PlayerGameResult!);
        return new PlayerDto
        {
            Id = entity.Id,
            Username = entity.Username,
            PlayerRatings = Utils.MapList(entity.PlayerRatings, _ratingMapper.ToDto),
            PlayerGameResult = playerResult
        };
    }
}