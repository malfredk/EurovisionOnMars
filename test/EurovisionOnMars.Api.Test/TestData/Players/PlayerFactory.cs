using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Api.Test.TestData.Players.PlayerRatings;
using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Test.TestData.Players;

public static class PlayerFactory
{
    public static Player CreateInitialPlayer()
    {
        var country = CountryFactory.CreateInitialCountry();
        var username = new Username(PlayerTestData.Username);
        return new Player(username, [country])
        {
            Id = PlayerTestData.Id
        };
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

    public static Player CreatePlayerAtEndOfGame()
    {
        var player = CreatePlayerThatHasRated();

        player.PlayerRatings[0].Country!.SetActualRank(new CountryPosition(CountryTestData.Rank));

        player.CalculateGamePoints();

        return player;
    }
}
