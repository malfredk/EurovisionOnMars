using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Mappers;

public interface IPlayerResultMapper
{
    public PlayerResultDto ToDto(PlayerResult entity);
}

public class PlayerResultMapper : IPlayerResultMapper
{
    public PlayerResultDto ToDto(PlayerResult entity)
    {
        return new PlayerResultDto
        {
            Id = entity.Id,
            Ranking = entity.Ranking,
            Score = entity.Score,
        };
    }
}