using EurovisionOnMars.Api.Features.PlayerRatings.Domain;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings.Domain;

public class PlayerRatingProcessorTest
{
    private readonly Mock<ILogger<PlayerRatingProcessor>> _loggerMock;
    private readonly Mock<ISpecialPointsValidator> _specialPointsValidator;
    private readonly Mock<IRankHandler> _rankHandler;
    private readonly Mock<ITieBreakDemotionHandler> _tieBreakDemotionHandler;

    private readonly PlayerRatingProcessor _processor;

    public PlayerRatingProcessorTest()
    {
        _loggerMock = new Mock<ILogger<PlayerRatingProcessor>>();
        _specialPointsValidator = new Mock<ISpecialPointsValidator>();
        _rankHandler = new Mock<IRankHandler>();
        _tieBreakDemotionHandler = new Mock<ITieBreakDemotionHandler>();

        _processor = new PlayerRatingProcessor(
            _loggerMock.Object,
            _specialPointsValidator.Object,
            _rankHandler.Object,
            _tieBreakDemotionHandler.Object
        );
    }

    [Fact]
    public void UpdatePlayerRating()
    {
        // arrange
        var request = Utils.CreateUpdatePlayerRatingRequest();
        var ratingToUpdate = Utils.CreateInitialPlayerRating();
        var otherRating = Utils.CreateInitialPlayerRating(1234);
        var ratings = new List<PlayerRating> { ratingToUpdate, otherRating };

        var ratingsWithCalculatedRank = new List<PlayerRating>
        {
            Utils.CreateInitialPlayerRating(78),
        }.ToList();
        _rankHandler.Setup(m => m.CalculateRanks(ratings))
            .Returns(ratingsWithCalculatedRank);

        // act
        _processor.UpdatePlayerRating(request, ratingToUpdate, ratings);

        // assert
        Assert.Equal(Utils.CATEGORY1_POINTS, ratingToUpdate.Category1Points);
        Assert.Equal(Utils.CATEGORY2_POINTS, ratingToUpdate.Category2Points);
        Assert.Equal(Utils.CATEGORY3_POINTS, ratingToUpdate.Category3Points);

        _specialPointsValidator
            .Verify(v => v.ValidateSpecialCategoryPoints(ratingToUpdate, ratings), Times.Once);
        _rankHandler
            .Verify(r => r.CalculateRanks(ratings), Times.Once);
        _tieBreakDemotionHandler
            .Verify(t => t.CalculateTieBreakDemotions(ratingToUpdate.Prediction, ratingsWithCalculatedRank, null), Times.Once);
    }

    [Fact]
    public void UpdatePlayerRating_UnchangedTotalPoints()
    {
        // arrange
        var request = Utils.CreateUpdatePlayerRatingRequest();
        var ratingToUpdate = Utils.CreatePlayerRating(
            category1Points: Utils.CATEGORY2_POINTS,
            category2Points: Utils.CATEGORY1_POINTS
        );
        var ratings = new List<PlayerRating> { ratingToUpdate };

        // act
        _processor.UpdatePlayerRating(request, ratingToUpdate, ratings);

        // assert
        Assert.Equal(Utils.CATEGORY1_POINTS, ratingToUpdate.Category1Points);
        Assert.Equal(Utils.CATEGORY2_POINTS, ratingToUpdate.Category2Points);
        Assert.Equal(Utils.CATEGORY3_POINTS, ratingToUpdate.Category3Points);

        _specialPointsValidator
            .Verify(v => v.ValidateSpecialCategoryPoints(ratingToUpdate, ratings), Times.Once);
        _rankHandler
            .Verify(r => r.CalculateRanks(It.IsAny<List<PlayerRating>>()), Times.Never);
        _tieBreakDemotionHandler
            .Verify(t => t.CalculateTieBreakDemotions(It.IsAny<Prediction>(), It.IsAny<List<PlayerRating>>(), It.IsAny<int>()), Times.Never);
    }
}
