using EurovisionOnMars.Entity.Players.GameResults;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Test.TestData.Players;

namespace EurovisionOnMars.Entity.Test.Players.GameResults;

public class PlayerTotalPointsCalculatorTest
{
    [Fact]
    public void Calculate_ValidRatingResults_SetsTotalPoints()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();

        var rating1Result = player.PlayerRatings[0].RatingGameResult;
        rating1Result.SetBonusPoints(new BonusPoints(-4));
        rating1Result.SetRankDifference(-10);

        var rating2Result = player.PlayerRatings[1].RatingGameResult;
        rating2Result.SetBonusPoints(new BonusPoints(0));
        rating2Result.SetRankDifference(3);

        // (-4 + abs(-10)) + (0 + abs(3))
        const int expectedTotalPoints = 9;

        // act
        PlayerTotalPointsCalculator.Calculate(
            player.PlayerRatings,
            player.PlayerGameResult);

        // assert
        Assert.Equal(
            expectedTotalPoints,
            player.PlayerGameResult.TotalPoints);
    }

    [Fact]
    public void Calculate_MissingBonusPoints_ThrowsInvalidOperationException()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();

        foreach (var rating in player.PlayerRatings)
        {
            rating.RatingGameResult.SetRankDifference(3);
        }

        // act
        var action = () => PlayerTotalPointsCalculator.Calculate(
            player.PlayerRatings,
            player.PlayerGameResult);

        // assert
        Assert.Throws<InvalidOperationException>(action);
        Assert.Null(player.PlayerGameResult.TotalPoints);
    }

    [Fact]
    public void Calculate_MissingRankDifference_ThrowsInvalidOperationException()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayerWith2Ratings();

        foreach (var rating in player.PlayerRatings)
        {
            rating.RatingGameResult.SetBonusPoints(
                new BonusPoints(0));
        }

        // act
        var action = () => PlayerTotalPointsCalculator.Calculate(
            player.PlayerRatings,
            player.PlayerGameResult);

        // assert
        Assert.Throws<InvalidOperationException>(action);
        Assert.Null(player.PlayerGameResult.TotalPoints);
    }
}