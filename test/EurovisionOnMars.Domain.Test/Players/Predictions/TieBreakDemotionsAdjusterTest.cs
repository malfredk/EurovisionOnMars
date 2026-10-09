using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Domain.Players.Predictions;
using EurovisionOnMars.Domain.Test.TestData.Game;

namespace EurovisionOnMars.Domain.Test.Players.Predictions;

public class TieBreakDemotionAdjusterTest
{
    const int OldTotalPoints = 10;
    const int NewTotalPoints = 2;
    private static readonly CountryPosition CalculatedRank = new(1);

    [Fact]
    public void Adjust_UpdatedRatingLeavesThreeWayTie_ReindexesRemainingDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        SetPrediction(
            updatedRating.Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 1);

        SetPrediction(
            ratings[1].Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 0);

        SetPrediction(
            ratings[2].Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 2);

        // updated rating leaves the tie
        updatedRating.Prediction.SetTotalGivenPoints(NewTotalPoints);

        // act
        TieBreakDemotionAdjuster.Adjust(
            updatedRating,
            ratings,
            OldTotalPoints);

        // assert
        Assert.Null(updatedRating.Prediction.TieBreakDemotion);

        AssertTieBreakDemotion(
            ratings[1],
            expectedDemotion: 0);

        AssertTieBreakDemotion(
            ratings[2],
            expectedDemotion: 1);
    }

    [Fact]
    public void Adjust_UpdatedRatingLeavesTwoWayTie_ResetsBothPredictionDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        SetPrediction(
            updatedRating.Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 0);

        SetPrediction(
            ratings[1].Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 1);

        // updated rating leaves the tie
        updatedRating.Prediction.SetTotalGivenPoints(NewTotalPoints);

        // act
        TieBreakDemotionAdjuster.Adjust(
            updatedRating,
            ratings,
            OldTotalPoints);

        // assert
        Assert.Null(updatedRating.Prediction.TieBreakDemotion);
        Assert.Null(ratings[1].Prediction.TieBreakDemotion);
    }

    [Fact]
    public void Adjust_UpdatedRatingJoinsUnresolvedTie_AllDemotionsAreNull()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        SetPrediction(
            updatedRating.Prediction,
            OldTotalPoints,
            tieBreakDemotion: 6);
        ratings[1].Prediction.SetTotalGivenPoints(NewTotalPoints);
        ratings[2].Prediction.SetTotalGivenPoints(NewTotalPoints);

        // updated rating joins tie
        updatedRating.Prediction.SetTotalGivenPoints(NewTotalPoints);

        // act
        TieBreakDemotionAdjuster.Adjust(
            updatedRating,
            ratings,
            OldTotalPoints);

        // assert
        Assert.All(
            ratings,
            rating => Assert.Null(
                rating.Prediction.TieBreakDemotion));
    }

    [Fact]
    public void Adjust_UpdatedRatingJoinsResolvedTie_ReindexesDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];
        updatedRating.Prediction.SetTotalGivenPoints(OldTotalPoints);
        updatedRating.Prediction.SetCalculatedRank(CalculatedRank);

        SetPrediction(
            ratings[1].Prediction,
            totalGivenPoints: NewTotalPoints,
            tieBreakDemotion: 1);

        SetPrediction(
            ratings[2].Prediction,
            totalGivenPoints: NewTotalPoints,
            tieBreakDemotion: 0);

        // updated rating joins tie
        updatedRating.Prediction.SetTotalGivenPoints(NewTotalPoints);

        // act
        TieBreakDemotionAdjuster.Adjust(
            updatedRating,
            ratings,
            OldTotalPoints);

        // assert
        AssertTieBreakDemotion(
            updatedRating,
            expectedDemotion: 0);

        AssertTieBreakDemotion(
            ratings[1],
            expectedDemotion: 2);

        AssertTieBreakDemotion(
            ratings[2],
            expectedDemotion: 1);
    }

    [Fact]
    public void Adjust_UpdatedRatingCreatesNewTie_AllDemotionsAreNull()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        SetPrediction(
            updatedRating.Prediction,
            OldTotalPoints,
            tieBreakDemotion: 6);
        ratings[1].Prediction.SetTotalGivenPoints(NewTotalPoints);

        // updated rating creates new tie
        updatedRating.Prediction.SetTotalGivenPoints(NewTotalPoints);

        // act
        TieBreakDemotionAdjuster.Adjust(
            updatedRating,
            ratings,
            OldTotalPoints);

        // assert
        Assert.All(
            ratings,
            rating => Assert.Null(
                rating.Prediction.TieBreakDemotion));
    }

    [Fact]
    public void Adjust_UpdatedRatingKeepsUniqueTotalPoints_NoChangeInDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];
        updatedRating.Prediction.SetTotalGivenPoints(OldTotalPoints);
        updatedRating.Prediction.SetCalculatedRank(CalculatedRank);

        var otherTotalPoints = 800;
        SetPrediction(
            ratings[1].Prediction,
            totalGivenPoints: otherTotalPoints,
            tieBreakDemotion: 1);

        SetPrediction(
            ratings[2].Prediction,
            totalGivenPoints: otherTotalPoints,
            tieBreakDemotion: 0);

        // updated rating gets new unique points
        updatedRating.Prediction.SetTotalGivenPoints(NewTotalPoints);

        // act
        TieBreakDemotionAdjuster.Adjust(
            updatedRating,
            ratings,
            OldTotalPoints);

        // assert
        Assert.Null(updatedRating.Prediction.TieBreakDemotion);

        AssertTieBreakDemotion(
            ratings[1],
            expectedDemotion: 1);

        AssertTieBreakDemotion(
            ratings[2],
            expectedDemotion: 0);
    }

    [Fact]
    public void Adjust_UpdatedRatingLeavesAndJoinsTie_ReindexesDemotions()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(5);
        var ratings = game.Players.First().PlayerRatings;

        var updatedRating = ratings[0];

        SetPrediction(
            updatedRating.Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 1);

        SetPrediction(
            ratings[1].Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 0);

        SetPrediction(
            ratings[2].Prediction,
            totalGivenPoints: OldTotalPoints,
            tieBreakDemotion: 2);

        SetPrediction(
            ratings[3].Prediction,
            totalGivenPoints: NewTotalPoints,
            tieBreakDemotion: 0);

        SetPrediction(
            ratings[4].Prediction,
            totalGivenPoints: NewTotalPoints,
            tieBreakDemotion: 1);

        // updated rating leaves tie to join other tie
        updatedRating.Prediction.SetTotalGivenPoints(NewTotalPoints);

        // act
        TieBreakDemotionAdjuster.Adjust(
            updatedRating,
            ratings,
            OldTotalPoints);

        // assert
        AssertTieBreakDemotion(
            updatedRating,
            expectedDemotion: 0);

        AssertTieBreakDemotion(
            ratings[1],
            expectedDemotion: 0);

        AssertTieBreakDemotion(
            ratings[2],
            expectedDemotion: 1);

        AssertTieBreakDemotion(
            ratings[3],
            expectedDemotion: 1);

        AssertTieBreakDemotion(
            ratings[4],
            expectedDemotion: 2);
    }

    private static void SetPrediction(
        Prediction prediction,
        int totalGivenPoints,
        int tieBreakDemotion)
    {
        prediction.SetTotalGivenPoints(totalGivenPoints);
        prediction.SetCalculatedRank(CalculatedRank);
        prediction.SetTieBreakDemotion(new TieBreakDemotion(tieBreakDemotion));
    }

    private static void AssertTieBreakDemotion(
        PlayerRating rating,
        int expectedDemotion)
    {
        var tieBreakDemotion = rating.Prediction.TieBreakDemotion;
        Assert.NotNull(tieBreakDemotion);

        Assert.Equal(
            expectedDemotion,
            tieBreakDemotion.Value);
    }
}