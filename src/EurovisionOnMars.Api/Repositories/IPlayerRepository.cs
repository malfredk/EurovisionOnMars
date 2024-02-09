using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Repositories;

public interface IPlayerRepository
{
    Task<ImmutableList<Player>> GetPlayers();
    Task<Player?> GetPlayer(int id);
    Task<Player?> GetPlayer(string username);
    Task<Player> CreatePlayer(string username);
}
