using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Game;
using EurovisionOnMars.Domain.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Test.TestData.Game;

public static class GameScenarioFactory
{
    public static GameScenario CreateInitialGameWith2PlayersAnd2Countries()
    {
        return new GameScenarioBuilder()
            .AddCountry(1)
            .AddCountry(2)
            .AddPlayer("malene")
            .AddPlayer("lars")
            .Build();
    }

    public static GameScenario CreateGameWherePlayersHaveRated()
    {
        var gameScenario = CreateInitialGameWith2PlayersAnd2Countries();

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

    public static GameScenario CreateGameWherePlayersHaveRatedAndActualCountryRanksAreSet()
    {
        var gameScenario = CreateGameWherePlayersHaveRated();
        
        // set actual ranks for countries
        gameScenario.Countries[0].SetActualRank(new CountryPosition(2));
        gameScenario.Countries[1].SetActualRank(new CountryPosition(1));

        return gameScenario;
    }

    public static GameScenario CreateCalculatedGame()
    {
        var gameScenario = CreateGameWherePlayersHaveRatedAndActualCountryRanksAreSet();
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
