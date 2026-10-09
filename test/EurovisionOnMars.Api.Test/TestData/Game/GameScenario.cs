using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;

namespace EurovisionOnMars.Api.Test.TestData.Game;

public sealed class GameScenario
{
    public IReadOnlyList<Country> Countries { get; }
    public IReadOnlyList<Player> Players { get; }

    public GameScenario(
        IReadOnlyList<Country> countries,
        IReadOnlyList<Player> players)
    {
        Countries = countries;
        Players = players;
    }
}