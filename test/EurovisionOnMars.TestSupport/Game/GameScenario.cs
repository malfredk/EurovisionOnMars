using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;

namespace EurovisionOnMars.TestSupport.Game;

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

    public void SetActualRanksForCountries()
    {
        for (int i = 0; i < Countries.Count; i++)
        {
            Countries[i].SetActualRank(new CountryPosition(i + 1));
        }
    }
}