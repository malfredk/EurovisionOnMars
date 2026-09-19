using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Entity.Test.Players.PlayerRatings;

public class PredictionTest
{
    [Fact]
    public void SetCalculatedRank_Valid()
    {
        // arrange
        var calculatedRank = new CountryPosition(2);
        var prediction = GetPrediction();

        // act
        prediction.SetCalculatedRank(calculatedRank);

        // assert
        Assert.Equal(calculatedRank, prediction.CalculatedRank);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(6)]
    public void SetTieBreakDemotion_Valid(int value)
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(new CountryPosition(20));

        var tieBreakDemotion = new TieBreakDemotion(value);

        // act
        prediction.SetTieBreakDemotion(tieBreakDemotion);

        // assert
        Assert.Equal(tieBreakDemotion, prediction.TieBreakDemotion);
    }

    [Fact]
    public void SetTieBreakDemotion_Null_Valid()
    {
        // arrange
        var prediction = GetPrediction();

        // act
        prediction.SetTieBreakDemotion(null);

        // assert
        Assert.Null(prediction.TieBreakDemotion);
    }

    [Fact]
    public void SetTieBreakDemotion_NoCalculatedRank_ThrowsInvalidOperationException()
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
    public void SetTieBreakDemotion_PredictedRankWouldBeInvalid_ThrowsInvalidOperationException()
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(new CountryPosition(20));

        var tieBreakDemotion = new TieBreakDemotion(7);

        // act and assert
        Assert.Throws<InvalidOperationException>(
            () => prediction.SetTieBreakDemotion(tieBreakDemotion));

        Assert.Null(prediction.TieBreakDemotion);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 21)]
    [InlineData(4, 24)]
    [InlineData(6, 26)]
    public void GetPredictedRank_WithTieBreakDemotion_ReturnsExpectedRank(
        int tieBreakDemotionValue,
        int expectedPredictedRankValue)
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(Utils.PREDICTION_CALCULATED_RANK);

        var tieBreakDemotion =
            new TieBreakDemotion(tieBreakDemotionValue);

        prediction.SetTieBreakDemotion(tieBreakDemotion);

        var expectedPredictedRank =
            new CountryPosition(expectedPredictedRankValue);

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
        prediction.SetCalculatedRank(Utils.PREDICTION_CALCULATED_RANK);

        // act
        var actualPredictedRank = prediction.GetPredictedRank();

        // assert
        Assert.Equal(
            Utils.PREDICTION_CALCULATED_RANK,
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
        var playerRating = Utils.CreateInitialPlayerRating();
        return playerRating.Prediction;
    }
}