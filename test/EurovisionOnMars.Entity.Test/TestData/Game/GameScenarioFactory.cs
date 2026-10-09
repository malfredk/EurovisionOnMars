using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Test.TestData.Countries;
using EurovisionOnMars.Entity.Test.TestData.Players;

namespace EurovisionOnMars.Entity.Test.TestData.Game;

public static class GameScenarioFactory
{
    public static GameScenario CreateInitialGame(
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

    public static GameScenario CreateInitialGameWithOnePlayer(int countriesCount)
    {
        var usernames = new List<string> { PlayerTestData.Username };
        var game = CreateInitialGame(usernames, countriesCount);
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
}
