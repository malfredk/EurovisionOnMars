using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.TestSupport.Countries;
using EurovisionOnMars.TestSupport.Players;
using System.Collections.Immutable;

namespace EurovisionOnMars.TestSupport.Game;

public static class GameScenarioFactory
{
    public static GameScenario CreateInitial(
        List<string> usernames,
        int countriesCount = 2
    )
    {
        var gameBuilder = new GameScenarioBuilder();

        for (int i = 1; i <= countriesCount; i++)
        {
            gameBuilder.AddCountry(i);
        }

        foreach (var username in usernames)
        {
            gameBuilder.AddPlayer(username);
        }

        return gameBuilder
            .Build();
    }

    public static GameScenario CreateInitialGame(int playersCount, int countriesCount)
    {
        var usernames = new List<string>();
        for (int i = 1; i <= playersCount; i++)
        {
            usernames.Add($"player{i}");
        }

        var game = CreateInitial(usernames, countriesCount);
        return game;
    }

    public static GameScenario CreateInitialGameWithOnePlayer(int countriesCount)
    {
        var usernames = new List<string> { PlayerTestData.Username };
        var game = CreateInitial(usernames, countriesCount);
        return game;
    }

    public static GameScenario CreateGameWithOnePlayerWithCalculatedAndActualRank(int countriesCount)
    {
        var game = CreateInitialGameWithOnePlayer(countriesCount);
        var ratings = game.Players.First().PlayerRatings;

        foreach (var rating in ratings)
        {
            rating.Prediction.SetCalculatedRank(new CountryPosition(3));
            rating.Country!.SetActualRank(new CountryPosition(CountryTestData.Rank));
        }
        return game;
    }

    public static GameScenario CreateWherePlayersHaveRatedAndActualCountryRanksAreSet()
    {
        var usernames = new List<string> { "malene", "lars" };
        var game = CreateInitial(usernames, 2);

        foreach (var player in game.Players)
        {
            SetPointsForPlayerRatings(player);
        }

        game.SetActualRanksForCountries();

        return game;
    }

    private static void SetPointsForPlayerRatings(Player player)
    {
        var points = new Points(1);
        foreach (var rating in player.PlayerRatings)
        {
            rating.SetPoints(points, points, points);
        }
    }
}
