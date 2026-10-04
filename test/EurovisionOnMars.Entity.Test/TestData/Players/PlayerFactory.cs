using EurovisionOnMars.Entity.Test.TestData.Countries;
using EurovisionOnMars.Entity.Test.TestData.Players.PlayerRatings;
using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Test.TestData.Players;

public static class PlayerFactory
{
    public static Player CreateInitialPlayer()
    {
        var countries = CountryFactory.CreateInitialSingletonList();
        var username = new Username(PlayerTestData.Username);
        return new Player(username, countries);
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

        var category1Points = new Points(PlayerRatingTestData.Category1Points);
        var category2Points = new Points(PlayerRatingTestData.Category2Points);
        var category3Points = new Points(PlayerRatingTestData.Category3Points);
        player.RateCountry(player.PlayerRatings[0].Id, category1Points, category2Points, category3Points);

        return player;
    }

    public static Player CreatePlayerWith2TiedRatings()
    {
        var player = CreateInitialPlayerWith2Ratings();

        var category1Points = new Points(PlayerRatingTestData.Category1Points);
        var category2Points = new Points(PlayerRatingTestData.Category2Points);
        var category3Points = new Points(PlayerRatingTestData.Category3Points);

        var rating1Id = player.PlayerRatings[0].Id;
        var rating2Id = player.PlayerRatings[1].Id;

        player.RateCountry(rating1Id, category1Points, category2Points, category3Points);
        player.RateCountry(rating2Id, category1Points, category2Points, category3Points);

        return player;
    }

    public static Player CreatePlayerWith2RatedAndDemotedAnd2InitialRatings()
    {
        var player = CreateInitialPlayerWith4Ratings();

        var category1Points = new Points(PlayerRatingTestData.Category1Points);
        var category2Points = new Points(PlayerRatingTestData.Category2Points);
        var category3Points = new Points(PlayerRatingTestData.Category3Points);

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
