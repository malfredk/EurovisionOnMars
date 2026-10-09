using EurovisionOnMars.Dto.Players.GetPlayer;
using EurovisionOnMars.Domain.Players;

namespace EurovisionOnMars.Api.Features.Players.GetPlayer;

public interface IPlayerMapper
{
    public PlayerDto ToDto(Player entity);
}

public class PlayerMapper : IPlayerMapper
{
    public PlayerDto ToDto(Player entity)
    {
        return new PlayerDto
        {
            Id = entity.Id,
            Username = entity.Username.Value
        };
    }
}