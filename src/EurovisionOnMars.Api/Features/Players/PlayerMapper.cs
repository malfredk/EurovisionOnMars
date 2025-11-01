using EurovisionOnMars.Dto.Players;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.Players;

public interface IPlayerMapper
{
    public PlayerResponseDto ToDto(Player entity);
}

public class PlayerMapper : IPlayerMapper
{
    public PlayerResponseDto ToDto(Player entity)
    {
        return new PlayerResponseDto
        {
            Id = entity.Id,
            Username = entity.Username
        };
    }
}