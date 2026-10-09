using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.Predictions;
using EurovisionOnMars.Entity.Test.TestData.Game;

namespace EurovisionOnMars.Entity.Test.Players.Predictions;

public class PredictionsCalculatorTest
{
    private const int OldTotalGivenPoints = 30;
    private static readonly TieBreakDemotion oldTieBreakDemotion = new(7);

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
    public void Calculate_TotalGivenPointsUnchanged_DoesNotRecalculateTiebreakDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];
        updatedRating.Prediction.SetTotalGivenPoints(OldTotalGivenPoints);
        updatedRating.Prediction.SetTieBreakDemotion(oldTieBreakDemotion);

        // act
        PredictionsCalculator.Calculate(
            updatedRating,
            ratings,
            OldTotalGivenPoints);

        // assert
        Assert.Equal(oldTieBreakDemotion, updatedRating.Prediction.TieBreakDemotion);
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
    public void Calculate_TotalGivenPointsChanged_RecalculatesTieBreakDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        updatedRating.Prediction.SetTotalGivenPoints(10);
        ratings[1].Prediction.SetTotalGivenPoints(30);
        ratings[2].Prediction.SetTotalGivenPoints(20);

        foreach (var rating in ratings)
        {
            rating.Prediction.SetTieBreakDemotion(oldTieBreakDemotion);
        }

        // act
        PredictionsCalculator.Calculate(
            updatedRating,
            ratings,
            OldTotalGivenPoints);

        // assert
        Assert.Null(updatedRating.Prediction.TieBreakDemotion);
        Assert.Null(ratings[1].Prediction.TieBreakDemotion);
        Assert.Null(ratings[2].Prediction.TieBreakDemotion);
    }
}