using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test;

public class PlayerRatingTest
{
    [Fact]
    public void SetPoints_Valid() {
        // arrange
        var rating = GetPlayerRating();
        var category1Points = Points.Create(2);
        var category2Points = Points.Create(5);
        var category3Points = Points.Create(3);

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
        var countries = new List<Country>{ new Country(1, "norge") }.ToImmutableList();
        var player = new Player("testuser", countries);

        return player.PlayerRatings.First();
    }
}
