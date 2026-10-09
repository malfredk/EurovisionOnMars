using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players.Predictions;
using EurovisionOnMars.Domain.Test.TestData.Game;

namespace EurovisionOnMars.Domain.Test.Players.Predictions;

public class PredictionsCalculatorTest
{
    private const int OldTotalGivenPoints = 30;
    private static readonly TieBreakDemotion OldTieBreakDemotion = new(7);

    [Fact]
    public void Calculate_TotalGivenPointsUnchanged_DoesNotRecalculateRanks()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;


        var updatedRating = ratings[0];
        updatedRating.Prediction.SetTotalGivenPoints(OldTotalGivenPoints);
        var oldRank = new CountryPosition(10);
        updatedRating.Prediction.SetCalculatedRank(oldRank);

        // act
        PredictionsCalculator.Calculate(
            updatedRating,
            ratings,
            OldTotalGivenPoints);

        // assert
        Assert.Equal(oldRank, updatedRating.Prediction.CalculatedRank);
        Assert.Null(ratings[1].Prediction.CalculatedRank);
        Assert.Null(ratings[2].Prediction.CalculatedRank);
    }

    [Fact]
    public void Calculate_TotalGivenPointsUnchanged_DoesNotAdjustTiebreakDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];
        updatedRating.Prediction.SetTotalGivenPoints(OldTotalGivenPoints);
        updatedRating.Prediction.SetCalculatedRank(new CountryPosition(1));
        updatedRating.Prediction.SetTieBreakDemotion(OldTieBreakDemotion);

        // act
        PredictionsCalculator.Calculate(
            updatedRating,
            ratings,
            OldTotalGivenPoints);

        // assert
        Assert.Equal(OldTieBreakDemotion, updatedRating.Prediction.TieBreakDemotion);
        Assert.Null(ratings[1].Prediction.TieBreakDemotion);
        Assert.Null(ratings[2].Prediction.TieBreakDemotion);
    }

    [Fact]
    public void Calculate_TotalGivenPointsChanged_CalculatesRanks()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        updatedRating.Prediction.SetTotalGivenPoints(10);
        ratings[1].Prediction.SetTotalGivenPoints(30);
        ratings[2].Prediction.SetTotalGivenPoints(20);

        // act
        PredictionsCalculator.Calculate(
            updatedRating,
            ratings,
            OldTotalGivenPoints);

        // assert
        Assert.Equal(3, updatedRating.Prediction.CalculatedRank!.Value);
        Assert.Equal(1, ratings[1].Prediction.CalculatedRank!.Value);
        Assert.Equal(2, ratings[2].Prediction.CalculatedRank!.Value);
    }

    [Fact]
    public void Calculate_TotalGivenPointsChanged_ResetsTieBreakDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        updatedRating.Prediction.SetTotalGivenPoints(10);
        ratings[1].Prediction.SetTotalGivenPoints(30);
        ratings[2].Prediction.SetTotalGivenPoints(20);

        updatedRating.Prediction.SetCalculatedRank(new CountryPosition(1));
        updatedRating.Prediction.SetTieBreakDemotion(OldTieBreakDemotion);

        // act
        PredictionsCalculator.Calculate(
            updatedRating,
            ratings,
            OldTotalGivenPoints);

        // assert
        Assert.All(
            ratings,
            rating => Assert.Null(
                rating.Prediction.TieBreakDemotion));
    }
}