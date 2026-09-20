namespace EurovisionOnMars.Entity.Test.Players.PlayerRatings;

public class PlayerRatingTest
{
    [Fact]
    public void SetPoints_Valid() {
        // arrange
        var rating = Utils.CreateInitialPlayerRating();

        // act
        rating.SetPoints(Utils.CATEGORY1_POINTS, Utils.CATEGORY2_POINTS, Utils.CATEGORY3_POINTS);    

        // assert
        Assert.Equal(Utils.CATEGORY1_POINTS, rating.Category1Points);
        Assert.Equal(Utils.CATEGORY2_POINTS, rating.Category2Points);
        Assert.Equal(Utils.CATEGORY3_POINTS, rating.Category3Points);
        Assert.Equal(24, rating.Prediction.TotalGivenPoints);
    }
}
