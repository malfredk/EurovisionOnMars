using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IPlayerGameResultMapper
{
    public PlayerGameResultDto ToDto(PlayerGameResult entity);
}

public class PlayerGameResultMapper : IPlayerGameResultMapper
{
    public PlayerGameResultDto ToDto(PlayerGameResult entity)
    {
        return new PlayerGameResultDto
        {
            Id = entity.Id,
            Rank = entity.Rank,
            TotalPoints = entity.TotalPoints,
        };
    }
}