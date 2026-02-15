using EurovisionOnMars.Api.Features.PlayerRatings.Domain;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings.Domain;

public class RankHandlerTest
{
    private readonly Mock<ILogger<RankHandler>> _loggerMock;
    private readonly IRankHandler _rankHandler;

    public RankHandlerTest()
    {
        _loggerMock = new Mock<ILogger<RankHandler>>();
        _rankHandler = new RankHandler(_loggerMock.Object);
    }

    [Fact]
    public void CalculateRanks()
    {
        // arrange
        var ratingMissingPoints = Utils.CreateInitialPlayerRating();
        var ratingMissingPoints2 = Utils.CreateInitialPlayerRating();
        var rating3Points = CreatePlayerRating(1);
        var rating5Points = CreatePlayerRating(3);
        var rating5Points2 = CreatePlayerRating(3);
        var rating5Points3 = CreatePlayerRating(3);
        var rating14Points = CreatePlayerRating(12);
        var rating14Points2 = CreatePlayerRating(12);

        var ratings = new List<PlayerRating>
        {
            ratingMissingPoints2,
            rating5Points2,
            rating14Points,
            rating5Points3,
            rating3Points,
            ratingMissingPoints,
            rating14Points2,
            rating5Points,
        };

        // act
        var rankedRatings = _rankHandler.CalculateRanks(ratings);

        // assert
        Assert.Equal(6, rankedRatings.Count);

        Assert.Equal(1, rating14Points.Prediction.CalculatedRank);
        Assert.Equal(1, rating14Points2.Prediction.CalculatedRank);
        Assert.Equal(3, rating5Points.Prediction.CalculatedRank);
        Assert.Equal(3, rating5Points2.Prediction.CalculatedRank);
        Assert.Equal(3, rating5Points3.Prediction.CalculatedRank);
        Assert.Equal(6, rating3Points.Prediction.CalculatedRank);
        Assert.Null(ratingMissingPoints.Prediction.CalculatedRank);
        Assert.Null(ratingMissingPoints2.Prediction.CalculatedRank);
    }

    private static PlayerRating CreatePlayerRating(int category1Points)
    {
        var rating = Utils.CreateInitialPlayerRating();
        rating.SetPoints(category1Points, 1, 1);
        return rating;
    }
}
