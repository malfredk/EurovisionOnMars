using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using EurovisionOnMars.Entity.Players.Predictions;
using EurovisionOnMars.Entity.Test.TestData.Players;

namespace EurovisionOnMars.Entity.Test.Players.Predictions;

public class PredictionTest
{
    private static readonly CountryPosition FirstPlace = new(1);

    private static readonly Points OnePoint = new(1);
    private static readonly Points TwoPoints = new(2);
    private static readonly Points ThreePoints = new(3);


    [Fact]
    public void Prediction_CreatesEmptyPredictionWithRatingConnection()
    {
        // arrange
        var rating = PlayerFactory.CreateInitialPlayer().PlayerRatings.First();

        // act
        var actualPrediction = new Prediction(rating);

        // assert
        Assert.Same(rating, actualPrediction.PlayerRating);
        Assert.Null(actualPrediction.TotalGivenPoints);
        Assert.Null(actualPrediction.CalculatedRank);
        Assert.Null(actualPrediction.TieBreakDemotion);
    }

    [Fact]
    public void SetTotalGivenPoints_SetsTotalGivenPoints()
    {
        // arrange
        var prediction = GetPrediction();
        var totalGivenPoints = 100;

        // act
        prediction.SetTotalGivenPoints(totalGivenPoints);

        // assert
        Assert.Equal(totalGivenPoints, prediction.TotalGivenPoints);
    }


    [Fact]
    public void SetCalculatedRank_SetsCalculatedRank()
    {
        // arrange
        var prediction = GetPrediction();

        // act
        prediction.SetCalculatedRank(FirstPlace);

        // assert
        Assert.Equal(FirstPlace, prediction.CalculatedRank);
    }

    [Fact]
    public void ResetTieBreakDemotion_NullsTieBreakDemotion()
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(FirstPlace);
        prediction.SetTieBreakDemotion(new TieBreakDemotion(1));

        // act
        prediction.ResetTieBreakDemotion();

        // assert
        Assert.Null(prediction.TieBreakDemotion);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(25)]
    public void SetTieBreakDemotion_Valid_SetsTieBreakDemotion(int value)
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(FirstPlace);

        var tieBreakDemotion = new TieBreakDemotion(value);

        // act
        prediction.SetTieBreakDemotion(tieBreakDemotion);

        // assert
        Assert.Equal(tieBreakDemotion, prediction.TieBreakDemotion);
    }

    [Fact]
    public void SetTieBreakDemotion_NoCalculatedRank_ThrowsAndDoesNotSet()
    {
        // arrange
        var prediction = GetPrediction();
        var tieBreakDemotion = new TieBreakDemotion(1);

        // act and assert
        Assert.Throws<InvalidOperationException>(
            () => prediction.SetTieBreakDemotion(tieBreakDemotion));

        Assert.Null(prediction.TieBreakDemotion);
    }

    [Fact]
    public void SetTieBreakDemotion_PredictedRankWouldBeInvalid_ThrowsAndDoesNotSet()
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(FirstPlace);

        var tieBreakDemotion = new TieBreakDemotion(26);

        // act and assert
        Assert.Throws<InvalidOperationException>(
            () => prediction.SetTieBreakDemotion(tieBreakDemotion));

        Assert.Null(prediction.TieBreakDemotion);
    }

    [Fact]
    public void GetPredictedRank_WithTieBreakDemotion_ReturnsPredictedRank()
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(FirstPlace);

        var tieBreakDemotion = new TieBreakDemotion(5);
        prediction.SetTieBreakDemotion(tieBreakDemotion);

        var expectedPredictedRank = new CountryPosition(6);

        // act
        var actualPredictedRank = prediction.GetPredictedRank();

        // assert
        Assert.Equal(expectedPredictedRank, actualPredictedRank);
    }

    [Fact]
    public void GetPredictedRank_NoTieBreakDemotion_ReturnsCalculatedRank()
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(FirstPlace);

        // act
        var actualPredictedRank = prediction.GetPredictedRank();

        // assert
        Assert.Equal(
            FirstPlace,
            actualPredictedRank);
    }

    [Fact]
    public void GetPredictedRank_NoCalculatedRank_ReturnsNull()
    {
        // arrange
        var prediction = GetPrediction();

        // act
        var actualPredictedRank = prediction.GetPredictedRank();

        // assert
        Assert.Null(actualPredictedRank);
    }

    private Prediction GetPrediction()
    {
        return PlayerFactory.CreateInitialPlayer()
            .PlayerRatings.First()
            .Prediction;
    }
}