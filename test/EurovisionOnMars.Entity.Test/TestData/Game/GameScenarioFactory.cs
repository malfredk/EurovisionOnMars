using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Game;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Test.TestData.Countries;
using EurovisionOnMars.Entity.Test.TestData.Players;
using EurovisionOnMars.Entity.Test.TestData.Players.PlayerRatings;

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

    public static GameScenario CreateGameWherePlayersHaveRated()
    {
        var gameScenario = CreateInitialGame();

        var points1 = new Points(1);
        var points2 = new Points(2);

        // first player has rated countries
        var player1 = gameScenario.Players[0];
        player1.RateCountry(player1.PlayerRatings[0].Id, points1, points1, points1);
        player1.RateCountry(player1.PlayerRatings[1].Id, points2, points2, points2);

        // second player has rated countries
        var player2 = gameScenario.Players[1];
        player2.RateCountry(player2.PlayerRatings[0].Id, points2, points2, points2);
        player2.RateCountry(player2.PlayerRatings[1].Id, points1, points1, points1);

        return gameScenario;
    }

    public static GameScenario CreateGameWhereActualCountryRanksAreSet()
    {
        var gameScenario = CreateGameWherePlayersHaveRated();
        
        // set actual ranks for countries
        gameScenario.Countries[0].SetActualRank(new CountryPosition(2));
        gameScenario.Countries[1].SetActualRank(new CountryPosition(1));

        return gameScenario;
    }

    public static GameScenario CreateInitialGameWithOneInactivePlayerAndRankedCountries()
    {
        var usernames = new List<string> { PlayerTestData.Username };
        var game = CreateInitialGame(usernames, 3);

        var countries = game.Countries;
        countries[0].SetActualRank(new CountryPosition(1));
        countries[1].SetActualRank(new CountryPosition(2));
        countries[2].SetActualRank(new CountryPosition(3));

        return game;
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

    public static GameScenario CreateGameWithOnePlayerWhoHasRatedAndRankedCountry()
    {
        var usernames = new List<string> { PlayerTestData.Username };
        var game = CreateInitialGame(usernames, 1);

        game.Players[0].PlayerRatings[0].SetPoints(
            new Points(PlayerRatingTestData.Category1Points),
            new Points(PlayerRatingTestData.Category2Points),
            new Points(PlayerRatingTestData.Category3Points)
            );

        var countries = game.Countries;
        countries[0].SetActualRank(new CountryPosition(CountryTestData.Rank));

        return game;
    }

    public static GameScenario CreateGameWithOnePlayerWhoHasRatedAndRankedCountry(int countriesCount)
    {
        var usernames = new List<string> { PlayerTestData.Username };
        var game = CreateInitialGame(usernames, countriesCount);

        game.Players[0].PlayerRatings[0].SetPoints(
            new Points(PlayerRatingTestData.Category1Points),
            new Points(PlayerRatingTestData.Category2Points),
            new Points(PlayerRatingTestData.Category3Points)
            );

        var countries = game.Countries;
        countries[0].SetActualRank(new CountryPosition(CountryTestData.Rank));

    }


    public static GameScenario CreateCalculatedGame()
    {
        var gameScenario = CreateGameWhereActualCountryRanksAreSet();
        var players = gameScenario.Players;

        foreach (var player in players)
        {
            player.CalculateGamePoints();
        }

        var playerRanksCalculator = new PlayerRanksCalculator();
        playerRanksCalculator.CalculatePlayerRanks(players);

        return gameScenario;
    }
}
