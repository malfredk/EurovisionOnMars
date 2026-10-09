using EurovisionOnMars.Dto.GameResults.GetPlayerResults;
using EurovisionOnMars.Domain.Players.GameResults;

namespace EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;

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
            Rank = entity.Rank?.Value,
            TotalPoints = entity.TotalPoints,
            PlayerUsername = player.Username.Value
        };
    }
}