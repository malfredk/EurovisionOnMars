using EurovisionOnMars.Entity.Players.Predictions;
using EurovisionOnMars.Entity.Test.TestData.Game;

namespace EurovisionOnMars.Entity.Test.Players.Predictions;

public class PredictionRanksCalculatorTest
{
    [Fact]
    public void Calculate_DifferentPoints_AssignsRanksByDescendingPoints()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        ratings[0].Prediction.SetTotalGivenPoints(10);
        ratings[1].Prediction.SetTotalGivenPoints(30);
        ratings[2].Prediction.SetTotalGivenPoints(20);

        // act
        PredictionRanksCalculator.Calculate(ratings);

        // assert
        Assert.Equal(3, ratings[0].Prediction.CalculatedRank!.Value);
        Assert.Equal(1, ratings[1].Prediction.CalculatedRank!.Value);
        Assert.Equal(2, ratings[2].Prediction.CalculatedRank!.Value);
    }

    [Fact]
    public void Calculate_SamePoints_AssignsSameRank()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        ratings[0].Prediction.SetTotalGivenPoints(30);
        ratings[1].Prediction.SetTotalGivenPoints(30);
        ratings[2].Prediction.SetTotalGivenPoints(20);

        // act
        PredictionRanksCalculator.Calculate(ratings);

        // assert
        Assert.Equal(1, ratings[0].Prediction.CalculatedRank!.Value);
        Assert.Equal(1, ratings[1].Prediction.CalculatedRank!.Value);
    }

    [Fact]
    public void Calculate_RankAfterTie_SkipsOccupiedRank()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        ratings[0].Prediction.SetTotalGivenPoints(30);
        ratings[1].Prediction.SetTotalGivenPoints(30);
        ratings[2].Prediction.SetTotalGivenPoints(20);

        // act
        PredictionRanksCalculator.Calculate(ratings);

        // assert
        Assert.Equal(1, ratings[0].Prediction.CalculatedRank!.Value);
        Assert.Equal(1, ratings[1].Prediction.CalculatedRank!.Value);
        Assert.Equal(3, ratings[2].Prediction.CalculatedRank!.Value);
    }

    [Fact]
    public void Calculate_NullTotalGivenPoints_DoesNotAssignRank()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        ratings[0].Prediction.SetTotalGivenPoints(30);
        ratings[1].Prediction.SetTotalGivenPoints(20);

        // ratings[2] remains unrated with TotalGivenPoints == null

        // act
        PredictionRanksCalculator.Calculate(ratings);

        // assert
        Assert.Equal(1, ratings[0].Prediction.CalculatedRank!.Value);
        Assert.Equal(2, ratings[1].Prediction.CalculatedRank!.Value);
        Assert.Null(ratings[2].Prediction.CalculatedRank);
    }

    [Fact]
    public void Calculate_MultipleTies_AssignsCompetitionRanks()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(5);
        var ratings = game.Players.First().PlayerRatings;

        ratings[0].Prediction.SetTotalGivenPoints(30);
        ratings[1].Prediction.SetTotalGivenPoints(30);
        ratings[2].Prediction.SetTotalGivenPoints(20);
        ratings[3].Prediction.SetTotalGivenPoints(20);
        ratings[4].Prediction.SetTotalGivenPoints(10);

        // act
        PredictionRanksCalculator.Calculate(ratings);

        // assert
        Assert.Equal(1, ratings[0].Prediction.CalculatedRank!.Value);
        Assert.Equal(1, ratings[1].Prediction.CalculatedRank!.Value);

        Assert.Equal(3, ratings[2].Prediction.CalculatedRank!.Value);
        Assert.Equal(3, ratings[3].Prediction.CalculatedRank!.Value);

        Assert.Equal(5, ratings[4].Prediction.CalculatedRank!.Value);
    }
}