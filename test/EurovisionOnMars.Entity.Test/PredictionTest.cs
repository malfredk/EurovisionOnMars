using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test;

public class PredictionTest
{
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(26)]
    [Theory]
    public void SetCalculatedRank_Valid(int calculatedRank)
    {
        // arrange
        var prediction = GetPrediction();

        // act
        prediction.SetCalculatedRank(calculatedRank);

        // assert
        Assert.Equal(calculatedRank, prediction.CalculatedRank);
    }

    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(27)]
    [Theory]
    public void SetCalculatedRank_Invalid(int calculatedRank)
    {
        // arrange
        var prediction = GetPrediction();

        // act and assert
        Assert.Throws<ArgumentException>(() => prediction.SetCalculatedRank(calculatedRank));
        Assert.Null(prediction.CalculatedRank);
    }

    [InlineData(0)]
    [InlineData(10)]
    [InlineData(26)]
    [InlineData(null)]
    [Theory]
    public void SetTieBreakDemotion_Valid(int? tieBreakDemotion)
    {
        // arrange
        var prediction = GetPrediction();

        // act
        prediction.SetTieBreakDemotion(tieBreakDemotion);

        // assert
        Assert.Equal(tieBreakDemotion, prediction.TieBreakDemotion);
    }

    [InlineData(-1)]
    [InlineData(27)]
    [Theory]
    public void SetTieBreakDemotion_Invalid(int tieBreakDemotion)
    {
        // arrange
        var prediction = GetPrediction();

        // act and assert
        Assert.Throws<ArgumentException>(() => prediction.SetTieBreakDemotion(tieBreakDemotion));
        Assert.Null(prediction.TieBreakDemotion);
    }

    [InlineData(10, null, 10)]
    [InlineData(10, 4, 14)]
    [Theory]
    public void GetPredictedRank(int calculatedRank, int? tieBreakDemotion, int expectedPredictedRank)
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetCalculatedRank(calculatedRank);
        prediction.SetTieBreakDemotion(tieBreakDemotion);

        // act
        var actualPredictedRank = prediction.GetPredictedRank();

        // assert
        Assert.Equal(expectedPredictedRank, actualPredictedRank);
    }

    [Fact]
    public void GetPredictedRank_NoCalculatedRank()
    {
        // arrange
        var prediction = GetPrediction();
        prediction.SetTieBreakDemotion(5);

        // act
        var actualPredictedRank = prediction.GetPredictedRank();

        // assert
        Assert.Null(actualPredictedRank);
    }

    private Prediction GetPrediction()
    {
        var countries = new List<Country> { new Country(1, "norge") }.ToImmutableList();
        var player = new Player("testuser", countries);

        return player.PlayerRatings.First().Prediction;
    }
}
