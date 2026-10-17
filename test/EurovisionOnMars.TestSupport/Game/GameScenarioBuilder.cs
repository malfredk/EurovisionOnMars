using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;
using System.Collections.Immutable;
using EurovisionOnMars.TestSupport.Countries;

namespace EurovisionOnMars.TestSupport.Game;

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
            CountryFactory.CreateInitial(number));

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
