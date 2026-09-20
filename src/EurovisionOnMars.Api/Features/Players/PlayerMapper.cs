using EurovisionOnMars.Dto.Players;
using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Api.Features.Players;

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