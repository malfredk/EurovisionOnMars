using EurovisionOnMars.Dto.PlayerGameResults;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

public interface IPlayerGameResultMapper
{
    public PlayerGameResultDto ToDto(PlayerGameResult entity);
}

public class PlayerGameResultMapper : IPlayerGameResultMapper
{
    public PlayerGameResultDto ToDto(PlayerGameResult entity)
    {
        var player = entity.Player ??
            throw new Exception("PlayerGameResult is missing Player.");

        return new PlayerGameResultDto
        {
            Rank = entity.Rank,
            TotalPoints = entity.TotalPoints,
            PlayerUsername = player.Username
        };
    }
}