using EurovisionOnMars.Entity.Test.TestData.Countries;
using EurovisionOnMars.Entity.Test.TestData.Players.PlayerRatings;
using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Test.TestData.Players;

public static class PlayerFactory
{

    private static readonly Points Category1Points = new(PlayerRatingTestData.Category1Points);
    private static readonly Points Category2Points = new(PlayerRatingTestData.Category2Points);
    private static readonly Points Category3Points = new(PlayerRatingTestData.Category3Points);

    public static Player CreateInitialPlayer()
    {
        var countries = CountryFactory.CreateInitialSingletonList();
        var username = new Username(PlayerTestData.Username);
        return new Player(username, countries);
    }

    public static Player CreatePlayerWithTotalPoints(int totalPoints)
    {
        var player = CreateInitialPlayer();
        player.PlayerGameResult.SetTotalPoints(totalPoints);
        return player;
    }

    public static Player CreateInitialPlayerWith2Ratings()
    {
        var countries = CountryFactory.CreateInitialListWith2Countries();
        var username = new Username(PlayerTestData.Username);
        return new Player(username, countries);
    }

    public static Player CreateInitialPlayerWith4Ratings()
    {
        var countries = CountryFactory.CreateInitialListWith4Countries();
        var username = new Username(PlayerTestData.Username);
        return new Player(username, countries);
    }

    public static Player CreatePlayerThatHasRated()
    {
        var player = CreateInitialPlayer();

        player.PlayerRatings[0].SetPoints(Category1Points, Category2Points, Category3Points);

        return player;
    }

    public static Player CreatePlayerWith2TiedRatings()
    {
        var player = CreateInitialPlayerWith2Ratings();

        var rating1 = player.PlayerRatings[0];
        var rating2 = player.PlayerRatings[1];

        rating1.SetPoints(Category1Points, Category2Points, Category3Points);
        rating1.SetPoints(Category1Points, Category2Points, Category3Points);

        return player;
    }

    public static Player CreatePlayerWith2RatedAndDemotedAnd2InitialRatings()
    {
        var player = CreateInitialPlayerWith4Ratings();

        var rating1 = player.PlayerRatings[0];
        var rating2 = player.PlayerRatings[1];

        player.RateCountry(rating1.Id, category1Points, category2Points, category3Points);
        player.RateCountry(rating2.Id, category1Points, category2Points, category3Points);

        player.ResolveTieBreak([rating2.Prediction.Id, rating1.Prediction.Id]);

        return player;
    }

    public static Player CreatePlayerAtEndOfGame()
    {
        var player = CreatePlayerThatHasRated();

        player.PlayerRatings[0].Country!.SetActualRank(new CountryPosition(CountryTestData.Rank));

        player.CalculateGamePoints();

        return player;
    }
}
