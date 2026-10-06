using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Test.TestData.Players;

namespace EurovisionOnMars.Entity.Test.Players.PlayerRatings;

public class RatingGameResultTest
{
    [Fact]
    public void Constructor_CreatesResultForPlayerRating()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings[0];

        // act
        var actualRatingGameResult = new RatingGameResult(rating);

        // assert
        Assert.Same(rating, actualRatingGameResult.PlayerRating);
        Assert.Null(actualRatingGameResult.RankDifference);
        Assert.Null(actualRatingGameResult.BonusPoints);
    }

    [Fact]
    public void SetRankDifference_SetsRankDifference()
    {
        // arrange
        var ratingGameResult = PlayerFactory.CreateInitialPlayer()
            .PlayerRatings[0].RatingGameResult;

        const int rankDifference = -3;

        // act
        ratingGameResult.SetRankDifference(rankDifference);

        // assert
        Assert.Equal(rankDifference, ratingGameResult.RankDifference);
    }

    [Fact]
    public void SetBonusPoints_SetsBonusPoints()
    {
        // arrange
        var ratingGameResult = PlayerFactory.CreateInitialPlayer()
            .PlayerRatings[0].RatingGameResult;

        var bonusPoints = new BonusPoints(-10);

        // act
        ratingGameResult.SetBonusPoints(bonusPoints);

        // assert
        Assert.Equal(bonusPoints, ratingGameResult.BonusPoints);
    }
}