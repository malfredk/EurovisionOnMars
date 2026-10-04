using EurovisionOnMars.Entity.Test.TestData.Countries;
using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test.TestData.Game;

public sealed class GameScenarioBuilder
{
    private readonly List<Country> _countries = [];
    private readonly List<Player> _players = [];

    public GameScenarioBuilder AddCountry(int number)
    {
        if (_players.Count > 0)
        {
            throw new InvalidOperationException(
                "Countries cannot be added after players have been added.");
        }

        _countries.Add(
            CountryFactory.CreateInitialCountry(number));

        return this;
    }

    public GameScenarioBuilder AddPlayer(string username)
    {
        var player = new Player(
            new Username(username),
            _countries.ToImmutableList());

        _players.Add(player);

        return this;
    }

    public GameScenario Build()
    {
        return new GameScenario(
            _countries.ToList(),
            _players.ToList());
    }
}
