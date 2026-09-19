using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test.Players;

public class PlayerRatingTest
{
    [Fact]
    public void SetPoints_Valid() {
        // arrange
        var rating = GetPlayerRating();
        var category1Points = new Points(2);
        var category2Points = new Points(5);
        var category3Points = new Points(3);

        // act
        rating.SetPoints(category1Points, category2Points, category3Points);    

        // assert
        Assert.Equal(category1Points, rating.Category1Points);
        Assert.Equal(category2Points, rating.Category2Points);
        Assert.Equal(category3Points, rating.Category3Points);
        Assert.Equal(10, rating.Prediction.TotalGivenPoints);
    }

    private PlayerRating GetPlayerRating()
    {
        var country = new Country(new CountryPosition(1), new CountryName("norge"));
        var countries = new List<Country>{ country }.ToImmutableList();
        var player = new Player(new Username("testuser"), countries);

        return player.PlayerRatings.First();
    }
}
