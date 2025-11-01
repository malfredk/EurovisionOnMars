using EurovisionOnMars.Dto.PlayerGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

public interface IPlayerGameResultMapper
{
    public PlayerGameResultResponseDto ToDto(PlayerGameResult entity);
}

public class PlayerGameResultMapper : IPlayerGameResultMapper
{
    public PlayerGameResultResponseDto ToDto(PlayerGameResult entity)
    {
        return new PlayerGameResultResponseDto
        {
            Id = entity.Id,
            Rank = entity.Rank,
            TotalPoints = entity.TotalPoints,
            PlayerUsername = entity.Player?.Username
        };
    }
}