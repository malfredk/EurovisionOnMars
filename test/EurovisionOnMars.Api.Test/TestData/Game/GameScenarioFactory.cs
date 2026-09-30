using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Game;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Test.TestData.Game;

public static class GameScenarioFactory
{
    public static GameScenario CreateInitialGame()
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
        var gameScenario = CreateInitialGame();

        var points1 = new Points(1);
        var points2 = new Points(2);

        // first player has rated countries
        gameScenario.Players[0].PlayerRatings[0].SetPoints(points1, points1, points1);
        gameScenario.Players[0].PlayerRatings[1].SetPoints(points2, points2, points2);

        // second player has rated countries
        gameScenario.Players[1].PlayerRatings[0].SetPoints(points2, points2, points2);
        gameScenario.Players[1].PlayerRatings[1].SetPoints(points1, points1, points1);

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
