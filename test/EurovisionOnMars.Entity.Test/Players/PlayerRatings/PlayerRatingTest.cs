using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Test.TestData.Countries;
using EurovisionOnMars.Entity.Test.TestData.Game;
using EurovisionOnMars.Entity.Test.TestData.Players;
using EurovisionOnMars.Entity.Test.TestData.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Test.Players.PlayerRatings;

public class PlayerRatingTest
{

    private static readonly Points Category1Points = new(PlayerRatingTestData.Category1Points);
    private static readonly Points Category2Points = new(PlayerRatingTestData.Category2Points);
    private static readonly Points Category3Points = new(PlayerRatingTestData.Category3Points);

    private static readonly CountryPosition CountryPosition10 = new(10);

    [Fact]
    public void Constructor_CreatesRatingForPlayerAndCountry()
    {
        // arrange
        var country = CountryFactory.CreateInitialCountry();
        var player = PlayerFactory.CreateInitialPlayer();

        // act
        var rating = new PlayerRating(player, country);

        // assert
        Assert.Same(player, rating.Player);
        Assert.Same(country, rating.Country);

        Assert.NotNull(rating.Prediction);
        Assert.NotNull(rating.RatingGameResult);

        Assert.Null(rating.Category1Points);
        Assert.Null(rating.Category2Points);
        Assert.Null(rating.Category3Points);
    }

    [Fact]
    public void SetPoints_SetsCategoryPoints()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings[0];

        // act
        rating.SetPoints(
            Category1Points,
            Category2Points,
            Category3Points);

        // assert
        Assert.Equal(Category1Points, rating.Category1Points);
        Assert.Equal(Category2Points, rating.Category2Points);
        Assert.Equal(Category3Points, rating.Category3Points);
    }

    [Fact]
    public void SetPoints_SetsTotalGivenPoints()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings[0];

        // act
        rating.SetPoints(
            Category1Points,
            Category2Points,
            Category3Points);

        // assert
        Assert.NotNull(rating.Prediction.TotalGivenPoints);
    }

    [InlineData(10, 0)]
    [InlineData(13, -3)]
    [InlineData(3, 7)]
    [Theory]
    public void CalculateRankDifference_WithPredictedRank_CalculatesDifference(int predictedRank, int expectedRankDifference)
    {
        // arrange
        var game = GameScenarioFactory.CreateInitalGameWithOnePlayer(1);
        var rating = game.Players[0].PlayerRatings[0];

        rating.Prediction.SetCalculatedRank(new CountryPosition(predictedRank));

        game.Countries[0].SetActualRank(CountryPosition10);

        // act
        rating.CalculateRankDifference();

        // assert
        Assert.Equal(
            expectedRankDifference,
            rating.RatingGameResult.RankDifference);
    }

    [Fact]
    public void CalculateRankDifference_WithoutPredictedRank_SetsPenalty()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitalGameWithOnePlayer(1);
        var rating = game.Players[0].PlayerRatings[0];

        game.Countries[0].SetActualRank(CountryPosition10);

        // act
        rating.CalculateRankDifference();

        // assert
        Assert.Equal(
            30,
            rating.RatingGameResult.RankDifference);
    }

    [Fact]
    public void CalculateRankDifference_MissingActualRank_ThrowsException()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings[0];

        // act
        var action = () => rating.CalculateRankDifference();

        // assert
        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void CalculateBonusPoints_ExactAndUniquePrediction_SetsBonusPoints()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings.First();
        rating.Prediction.SetCalculatedRank(CountryPosition10);
        rating.RatingGameResult.SetRankDifference(0);

        // act
        rating.CalculateBonusPoints(
            hasUniquePredictedRank: true);

        // assert
        var actualBonusPoints = rating.RatingGameResult.BonusPoints;
        Assert.NotNull(actualBonusPoints);
        Assert.NotEqual(0, actualBonusPoints.Value);
    }

    [Fact]
    public void CalculateBonusPoints_ExactButNotUniquePrediction_SetsZeroBonusPoints()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings.First();
        rating.Prediction.SetCalculatedRank(CountryPosition10);
        rating.RatingGameResult.SetRankDifference(0);

        // act
        rating.CalculateBonusPoints(
            hasUniquePredictedRank: false);

        // assert
        Assert.Equal(
            new BonusPoints(0),
            rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void CalculateBonusPoints_UniqueButIncorrectPrediction_SetsZeroBonusPoints()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings.First();
        rating.Prediction.SetCalculatedRank(CountryPosition10);
        rating.RatingGameResult.SetRankDifference(1);

        // act
        rating.CalculateBonusPoints(
            hasUniquePredictedRank: true);

        // assert
        Assert.Equal(
            new BonusPoints(0),
            rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void CalculateBonusPoints_MissingPredictedRank_SetsZeroBonusPoints()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings.First();

        // act
        rating.CalculateBonusPoints(
            hasUniquePredictedRank: true);

        // assert
        Assert.Equal(
            new BonusPoints(0),
            rating.RatingGameResult.BonusPoints);
    }
}