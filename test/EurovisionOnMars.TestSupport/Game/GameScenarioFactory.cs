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
}
