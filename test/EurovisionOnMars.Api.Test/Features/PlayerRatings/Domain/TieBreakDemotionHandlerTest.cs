using EurovisionOnMars.Api.Features.PlayerRatings.Domain;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings.Domain;

public class TieBreakDemotionHandlerTest
{
    private readonly Mock<ILogger<TieBreakDemotionHandler>> _loggerMock;
    private readonly TieBreakDemotionHandler _handler;

    public TieBreakDemotionHandlerTest()
    {
        _loggerMock = new Mock<ILogger<TieBreakDemotionHandler>>();
        _handler = new TieBreakDemotionHandler(_loggerMock.Object);
    }

    [Fact]
    public void CalculateTieBreakDemotions_AdjustOnlyOldAndNewGroup()
    {
        // arrange
        var newPoints = 8;
        var rating = CreatePlayerRating(newPoints, tieBreakDemotion: 1);
        var prediction = rating.Prediction;

        var newGroupRating1 = CreatePlayerRating(newPoints, tieBreakDemotion: 0);
        var newGroupRating2 = CreatePlayerRating(newPoints, tieBreakDemotion: 5);
        var newGroupRating3 = CreatePlayerRating(newPoints, tieBreakDemotion: 3);

        var oldPoints = 4;
        var oldGroupRating1 = CreatePlayerRating(oldPoints, tieBreakDemotion: 4);
        var oldGroupRating2 = CreatePlayerRating(oldPoints, tieBreakDemotion: 7);

        var otherRating1 = CreatePlayerRating(givenPoints: 1, tieBreakDemotion: 2);
        var otherRating2 = CreatePlayerRating(givenPoints: 1, tieBreakDemotion: null);
        var otherRating3 = CreatePlayerRating(givenPoints: 10, tieBreakDemotion: 7);
        var otherRating4 = Utils.CreateInitialPlayerRating();

        var allRatings = new List<PlayerRating>
        {
            otherRating4,
            rating,
            newGroupRating1,
            oldGroupRating1,
            newGroupRating3,
            oldGroupRating2,
            otherRating1,
            newGroupRating2,
            otherRating2,
            otherRating3
        };

        // act
        _handler.CalculateTieBreakDemotions(prediction, allRatings, oldPoints+2);

        // assert
        Assert.Equal(0, prediction.TieBreakDemotion);
        
        Assert.Equal(1, newGroupRating1.Prediction.TieBreakDemotion);
        Assert.Equal(2, newGroupRating3.Prediction.TieBreakDemotion);
        Assert.Equal(3, newGroupRating2.Prediction.TieBreakDemotion);

        Assert.Equal(0, oldGroupRating1.Prediction.TieBreakDemotion);
        Assert.Equal(1, oldGroupRating2.Prediction.TieBreakDemotion);

        Assert.Equal(2, otherRating1.Prediction.TieBreakDemotion);
        Assert.Null(otherRating2.Prediction.TieBreakDemotion);
        Assert.Equal(7, otherRating3.Prediction.TieBreakDemotion);
        Assert.Null(otherRating4.Prediction.TieBreakDemotion);
    }

    [Fact]
    public void CalculateTieBreakDemotions_NoOldGroup_DoesNotThrow()
    {
        // arrange
        var newPoints = 8;
        var rating = CreatePlayerRating(newPoints, tieBreakDemotion: 1);
        var prediction = rating.Prediction;

        var otherGroupRating = CreatePlayerRating(4, tieBreakDemotion: 4);

        var allRatings = new List<PlayerRating>
        {
            rating,
            otherGroupRating
        };

        // act
        var ex = Record.Exception(() =>
            _handler.CalculateTieBreakDemotions(prediction, allRatings, 34));

        // assert
        Assert.Null(ex);
    }

    [Fact]
    public void CalculateTieBreakDemotions_SingletonOldGroup_ResetOldGroup()
    {
        // arrange
        var newPoints = 8;
        var rating = CreatePlayerRating(newPoints, tieBreakDemotion: 1);
        var prediction = rating.Prediction;

        var oldPoints = 4;
        var oldGroupRating = CreatePlayerRating(oldPoints, tieBreakDemotion: 4);

        var allRatings = new List<PlayerRating>
        {
            rating,
            oldGroupRating
        };

        // act
        _handler.CalculateTieBreakDemotions(prediction, allRatings, oldPoints + 2);

        // assert
        Assert.Null(prediction.TieBreakDemotion);
    }


    [Fact]
    public void CalculateTieBreakDemotions_TiedOldGroup_DoesNotAdjustOldGroup()
    {
        // arrange
        var newPoints = 8;
        var rating = CreatePlayerRating(newPoints, tieBreakDemotion: 1);
        var prediction = rating.Prediction;

        var oldPoints = 4;
        var oldGroupRating1 = CreatePlayerRating(oldPoints, tieBreakDemotion: null);
        var oldGroupRating2 = CreatePlayerRating(oldPoints, tieBreakDemotion: null);

        var allRatings = new List<PlayerRating>
        {
            rating,
            oldGroupRating1,
            oldGroupRating2
        };

        // act
        _handler.CalculateTieBreakDemotions(prediction, allRatings, oldPoints + 2);

        // assert
        Assert.Null(oldGroupRating1.Prediction.TieBreakDemotion);
        Assert.Null(oldGroupRating2.Prediction.TieBreakDemotion);
    }

    [Fact]
    public void CalculateTieBreakDemotions_SingletonNewGroup_ResetNewPrediction()
    {
        // arrange
        var newPoints = 8;
        var rating = CreatePlayerRating(newPoints, tieBreakDemotion: 1);
        var prediction = rating.Prediction;

        var otherGroupRating = CreatePlayerRating(4, tieBreakDemotion: 4);

        var allRatings = new List<PlayerRating>
        {
            rating,
            otherGroupRating
        };

        // act
        _handler.CalculateTieBreakDemotions(prediction, allRatings, 67);

        // assert
        Assert.Null(prediction.TieBreakDemotion);
    }

    [Fact]
    public void CalculateTieBreakDemotions_TiedNewGroup_DoesNotAdjustNewGroup()
    {
        // arrange
        var newPoints = 8;
        var rating = CreatePlayerRating(newPoints, tieBreakDemotion: 1);
        var prediction = rating.Prediction;

        var newGroupRating1 = CreatePlayerRating(newPoints, tieBreakDemotion: null);
        var newGroupRating2 = CreatePlayerRating(newPoints, tieBreakDemotion: null);

        var allRatings = new List<PlayerRating>
        {
            rating,
            newGroupRating2,
            newGroupRating1
        };

        // act
        _handler.CalculateTieBreakDemotions(prediction, allRatings, 2);

        // assert
        Assert.Null(prediction.TieBreakDemotion);
        Assert.Null(newGroupRating1.Prediction.TieBreakDemotion);
        Assert.Null(newGroupRating2.Prediction.TieBreakDemotion);
    }

    [Fact]
    public void CalculateTieBreakDemotions_NullOldPoints_DoesNotAdjustNullPointRatings()
    {
        // arrange
        var newPoints = 8;
        var rating = CreatePlayerRating(newPoints, tieBreakDemotion: 1);
        var prediction = rating.Prediction;

        var nullPointRating = Utils.CreateInitialPlayerRating();
        nullPointRating.Prediction.SetTieBreakDemotion(4);

        var allRatings = new List<PlayerRating>
        {
            rating,
            nullPointRating
        };

        // act
        _handler.CalculateTieBreakDemotions(prediction, allRatings, null);

        // assert
        Assert.Equal(4, nullPointRating.Prediction.TieBreakDemotion);
    }

    public static PlayerRating CreatePlayerRating(
        int givenPoints,
        int? tieBreakDemotion = null
    )
    {
        var rating = Utils.CreatePlayerRating(1, 1, givenPoints);
        rating.Prediction.SetTieBreakDemotion(tieBreakDemotion);
        return rating;
    }
}
