using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using System.Collections.Immutable;

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

    [InlineData(null, 10)]
    [InlineData(4, 14)]
    [Theory]
    public void GetPredictedRank(int? tieBreakDemotion, int expectedPredictedRankValue)
    {
        // arrange
        var prediction = GetPrediction();

        var calculatedRank = new CountryPosition(10);
        prediction.SetCalculatedRank(calculatedRank);

        prediction.SetTieBreakDemotion(tieBreakDemotion);

        var expectedPredictedRank = new CountryPosition(expectedPredictedRankValue);

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
        var countries = new List<Country> { new Country(new CountryPosition(1), new CountryName("norge")) }.ToImmutableList();
        var player = new Player(new Username("testuser"), countries);

        return player.PlayerRatings.First().Prediction;
    }
}
