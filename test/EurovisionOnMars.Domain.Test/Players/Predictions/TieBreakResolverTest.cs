using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players.Predictions;
using EurovisionOnMars.Domain.Test.TestData.Game;

namespace EurovisionOnMars.Domain.Test.Players.Predictions;

public class TieBreakResolverTest
{
    [Fact]
    public void Resolve_ValidOrderedPredictionIds_SetsTieBreakDemotionsInRequestedOrder()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var predictions = game.Players
            .First()
            .PlayerRatings
            .Select(r => r.Prediction)
            .ToList();

        SetPrediction(predictions[0], id: 101, calculatedRank: 1);
        SetPrediction(predictions[1], id: 102, calculatedRank: 1);
        SetPrediction(predictions[2], id: 103, calculatedRank: 1);

        var orderedPredictionIds = new List<int>
        {
            103,
            101,
            102
        };

        // act
        TieBreakResolver.Resolve(
            predictions,
            orderedPredictionIds);

        // assert
        AssertTieBreakDemotion(predictions[2], 0);
        AssertTieBreakDemotion(predictions[0], 1);
        AssertTieBreakDemotion(predictions[1], 2);
    }

    [Fact]
    public void Resolve_ValidOrderedPredicitonIds_ChangesTieBreakDemotion()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var predictions = game.Players
            .First()
            .PlayerRatings
            .Select(r => r.Prediction)
            .ToList();

        SetPrediction(predictions[0], id: 101, calculatedRank: 1);
        predictions[0].SetTieBreakDemotion(new TieBreakDemotion(2));
        
        SetPrediction(predictions[1], id: 102, calculatedRank: 1);
        predictions[1].SetTieBreakDemotion(new TieBreakDemotion(0));
        
        SetPrediction(predictions[2], id: 103, calculatedRank: 1);
        predictions[2].SetTieBreakDemotion(new TieBreakDemotion(1));

        var orderedPredictionIds = new List<int>
        {
            103,
            101,
            102
        };

        // act
        TieBreakResolver.Resolve(
            predictions,
            orderedPredictionIds);

        // assert
        AssertTieBreakDemotion(predictions[2], 0);
        AssertTieBreakDemotion(predictions[0], 1);
        AssertTieBreakDemotion(predictions[1], 2);
    }

    [Theory]
    [InlineData()]
    [InlineData(101)]
    public void Resolve_LessThanTwoPredictionIds_ThrowsAndDoesNotSet(
        params int[] ids)
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);
        var predictions = game.Players
            .First()
            .PlayerRatings
            .Select(r => r.Prediction)
            .ToList();

        SetPrediction(predictions[0], id: 101, calculatedRank: 1);
        SetPrediction(predictions[1], id: 102, calculatedRank: 1);

        var orderedPredictionIds = ids.ToList();

        // act
        var action = () => TieBreakResolver.Resolve(
            predictions,
            orderedPredictionIds);

        // assert
        Assert.Throws<ArgumentException>(action);

        Assert.All(
            predictions,
            prediction => Assert.Null(
                prediction.TieBreakDemotion));
    }

    [Fact]
    public void Resolve_DuplicatePredictionIds_ThrowsAndDoesNotSet()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);
        var predictions = game.Players
            .First()
            .PlayerRatings
            .Select(r => r.Prediction)
            .ToList();

        SetPrediction(predictions[0], id: 101, calculatedRank: 1);
        SetPrediction(predictions[1], id: 102, calculatedRank: 1);

        var orderedPredictionIds = new List<int>
        {
            101,
            101
        };

        // act
        var action = () => TieBreakResolver.Resolve(
            predictions,
            orderedPredictionIds);

        // assert
        Assert.Throws<ArgumentException>(action);

        Assert.All(
            predictions,
            prediction => Assert.Null(
                prediction.TieBreakDemotion));
    }

    [Fact]
    public void Resolve_IncompleteTiedPredictionGroup_ThrowsAndDoesNotSet()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var predictions = game.Players
            .First()
            .PlayerRatings
            .Select(r => r.Prediction)
            .ToList();

        SetPrediction(predictions[0], id: 101, calculatedRank: 1);
        SetPrediction(predictions[1], id: 102, calculatedRank: 1);
        SetPrediction(predictions[2], id: 103, calculatedRank: 1);

        // prediction 103 belongs to the same tie, but has been omitted from the request.
        var orderedPredictionIds = new List<int>
        {
            101,
            102
        };

        // act
        var action = () => TieBreakResolver.Resolve(
            predictions,
            orderedPredictionIds);

        // assert
        Assert.Throws<ArgumentException>(action);

        Assert.All(
            predictions,
            prediction => Assert.Null(
                prediction.TieBreakDemotion));
    }

    [Fact]
    public void Resolve_PredictionsFromDifferentRanks_ThrowsAndDoesNotSet()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(3);
        var predictions = game.Players
            .First()
            .PlayerRatings
            .Select(r => r.Prediction)
            .ToList();

        SetPrediction(predictions[0], id: 101, calculatedRank: 1);
        SetPrediction(predictions[1], id: 102, calculatedRank: 1);
        SetPrediction(predictions[2], id: 103, calculatedRank: 3);

        var orderedPredictionIds = new List<int>
        {
            101,
            103
        };

        // act
        var action = () => TieBreakResolver.Resolve(
            predictions,
            orderedPredictionIds);

        // assert
        Assert.Throws<ArgumentException>(action);

        Assert.All(
            predictions,
            prediction => Assert.Null(
                prediction.TieBreakDemotion));
    }

    [Fact]
    public void Resolve_PredictionWithoutCalculatedRank_ThrowsAndDoesNotSet()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGameWithOnePlayer(2);
        var predictions = game.Players
            .First()
            .PlayerRatings
            .Select(r => r.Prediction)
            .ToList();

        predictions[0].Id = 101;

        SetPrediction(
            predictions[1],
            id: 102,
            calculatedRank: 1);

        var orderedPredictionIds = new List<int>
        {
            101,
            102
        };

        // act
        var action = () => TieBreakResolver.Resolve(
            predictions,
            orderedPredictionIds);

        // assert
        Assert.Throws<ArgumentException>(action);

        Assert.All(
            predictions,
            prediction => Assert.Null(
                prediction.TieBreakDemotion));
    }

    private static void SetPrediction(
        Prediction prediction,
        int id,
        int calculatedRank)
    {
        prediction.Id = id;
        prediction.SetCalculatedRank(
            new CountryPosition(calculatedRank));
    }

    private static void AssertTieBreakDemotion(
        Prediction prediction,
        int expectedValue)
    {
        Assert.NotNull(prediction.TieBreakDemotion);

        Assert.Equal(
            expectedValue,
            prediction.TieBreakDemotion.Value);
    }
}