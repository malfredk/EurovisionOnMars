using EurovisionOnMars.Api.Features;
using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings;

public class PlayerRatingServiceTest
{
    private readonly Mock<IPlayerRatingRepository> _repositoryMock;
    private readonly Mock<IRatingTimeValidator> _ratingTimeValidatorMock;
    private readonly Mock<ILogger<PlayerRatingService>> _loggerMock;
    private readonly Mock<ISpecialPointsValidator> _specialPointsValidatorMock;
    private readonly Mock<IRankHandler> _rankHandlerMock;
    private readonly Mock<ITieBreakDemotionHandler> _tieBreakDemotionHandlerMock;
    private readonly PlayerRatingService _service;

    public PlayerRatingServiceTest()
    {
        _repositoryMock = new Mock<IPlayerRatingRepository>();
        _ratingTimeValidatorMock = new Mock<IRatingTimeValidator>();
        _loggerMock = new Mock<ILogger<PlayerRatingService>>();
        _specialPointsValidatorMock = new Mock<ISpecialPointsValidator>();
        _rankHandlerMock = new Mock<IRankHandler>();
        _tieBreakDemotionHandlerMock = new Mock<ITieBreakDemotionHandler>();

        _service = new PlayerRatingService(
            _repositoryMock.Object, 
            _ratingTimeValidatorMock.Object,
            _loggerMock.Object, 
            _specialPointsValidatorMock.Object,
            _rankHandlerMock.Object,
            _tieBreakDemotionHandlerMock.Object
            );
    }

    // tests for GetAllPlayerRatings

    [Fact]
    public async Task GetAllPlayerRatings()
    {
        // arrange
        ImmutableList<PlayerRating> expectedRatings = [Utils.CreateInitialPlayerRating()];
        _repositoryMock.Setup(r => r.GetAllPlayerRatings())
            .ReturnsAsync(expectedRatings);

        // act
        var actualRatings = await _service.GetAllPlayerRatings();

        // assert
        Assert.Equal(expectedRatings, actualRatings);
        _ratingTimeValidatorMock.Verify(m => m.EnsureRatingIsOpen(), Times.Never());
        _repositoryMock.Verify(r => r.GetAllPlayerRatings(), Times.Once());
    }

    // tests for GetPlayerRatingsByPlayerId

    [Fact]
    public async Task GetRatingsByPlayerId()
    {
        // arrange
        var rating1 = CreatePlayerRating(1, 26);
        var rating2 = CreateInitialPlayerRating(10);
        var rating3 = CreateInitialPlayerRating(8);
        var rating4 = CreatePlayerRating(26, 1);
        ImmutableList<PlayerRating> ratings = [rating1, rating2, rating3, rating4];
        ImmutableList<PlayerRating> expectedRatings = [rating4, rating1, rating3, rating2];

        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID))
            .ReturnsAsync(ratings);

        // act
        var actualRatings = await _service.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID);

        // assert
        Assert.Equal(expectedRatings, actualRatings);

        _ratingTimeValidatorMock.Verify(m => m.EnsureRatingIsOpen(), Times.Never());
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID), Times.Once());
    }

    [Fact]
    public async Task GetRatingsByPlayer_NoResults()
    {
        // arrange
        var ratings = new List<PlayerRating>() { }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID))
            .ReturnsAsync(ratings);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID)
        );
        
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID), Times.Once());
    }

    // tests for UpdatePlayerRating

    [Fact]
    public async Task UpdatePlayerRating()
    {
        // arrange
        var request = CreateRequestDto();

        var rating = Utils.CreateInitialPlayerRating();
        var otherRating = Utils.CreateInitialPlayerRating(1222);
        var ratings = new List<PlayerRating>() { rating, otherRating };
        _repositoryMock.Setup(m => m.GetPlayerRatingsForPlayer(Utils.RATING_ID))
            .ReturnsAsync(ratings);

        var updatedRating = Utils.CreateInitialPlayerRating(66);
        var otherUpdatedRating = Utils.CreateInitialPlayerRating(89);
        var updatedRatings = new List<PlayerRating> { updatedRating, otherUpdatedRating };
        _rankHandlerMock.Setup(m => m.CalculateRanks(ratings))
            .Returns(updatedRatings);

        // act
        await _service.UpdatePlayerRating(Utils.RATING_ID, request);

        // assert
        _ratingTimeValidatorMock
            .Verify(m => m.EnsureRatingIsOpen(), Times.Once);

        _repositoryMock
            .Verify(m => m.GetPlayerRatingsForPlayer(Utils.RATING_ID), Times.Once);

        Assert.Equal(CATEGORY1_POINTS, rating.Category1Points);
        Assert.Equal(CATEGORY2_POINTS, rating.Category2Points);
        Assert.Equal(CATEGORY3_POINTS, rating.Category3Points);
        _specialPointsValidatorMock
            .Verify(m => m.ValidateSpecialCategoryPoints(rating, ratings), Times.Once);

        _rankHandlerMock
            .Verify(m => m.CalculateRanks(ratings), Times.Once);
        _tieBreakDemotionHandlerMock
            .Verify(
                m => m.CalculateTieBreakDemotions(rating.Prediction, updatedRatings, It.IsAny<SimplePrediction>()), 
                Times.Once
            );

        _repositoryMock
            .Verify(m => m.UpdateRating(updatedRating), Times.Once);
        _repositoryMock
            .Verify(m => m.UpdateRating(otherUpdatedRating), Times.Once);
    }

    [Fact]
    public async Task UpdatePlayerRating_RatingTimeValidatorThrowsException()
    {
        // arrange
        var request = CreateRequestDto();

        _ratingTimeValidatorMock.Setup(m => m.EnsureRatingIsOpen())
            .Throws<RatingIsClosedException>();

        // act and assert
        await Assert.ThrowsAsync<RatingIsClosedException>(() => _service.UpdatePlayerRating(Utils.RATING_ID, request));

        _repositoryMock
            .Verify(m => m.GetPlayerRatingsForPlayer(It.IsAny<int>()), Times.Never);
        _specialPointsValidatorMock
            .Verify(m => m.ValidateSpecialCategoryPoints(It.IsAny<PlayerRating>(), It.IsAny<IReadOnlyList<PlayerRating>>()), Times.Never);
        _rankHandlerMock
            .Verify(m => m.CalculateRanks(It.IsAny<IReadOnlyList<PlayerRating>>()), Times.Never);
        _repositoryMock
            .Verify(m => m.UpdateRating(It.IsAny<PlayerRating>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePlayerRating_SpecialPointsValidatorThrowsException()
    {
        // arrange
        var request = CreateRequestDto();

        var rating = Utils.CreateInitialPlayerRating();
        var otherRating = Utils.CreateInitialPlayerRating(1222);
        var ratings = new List<PlayerRating>() { rating, otherRating };
        _repositoryMock.Setup(m => m.GetPlayerRatingsForPlayer(Utils.RATING_ID))
            .ReturnsAsync(ratings);

        _specialPointsValidatorMock.Setup(m => m.ValidateSpecialCategoryPoints(rating, ratings))
            .Throws<ArgumentException>();

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdatePlayerRating(Utils.RATING_ID, request));

        _rankHandlerMock
            .Verify(m => m.CalculateRanks(It.IsAny<IReadOnlyList<PlayerRating>>()), Times.Never);
        _repositoryMock
            .Verify(m => m.UpdateRating(It.IsAny<PlayerRating>()), Times.Never);
    }

    // helpers

    private static PlayerRating CreatePlayerRating(int countryNumber, int predictionRank)
    {
        var rating = CreateInitialPlayerRating(countryNumber);
        rating.Prediction.SetCalculatedRank((int)predictionRank);
        return rating;
    }

    private static PlayerRating CreateInitialPlayerRating(int countryNumber)
    {
        var country = Utils.CreateInitialCountry(countryNumber);
        var player = Utils.CreateInitialPlayer(country);

        return player.PlayerRatings.FirstOrDefault()!;
    }

    private static UpdatePlayerRatingRequestDto CreateRequestDto()
    {
        return new UpdatePlayerRatingRequestDto()
        {
            Category1Points = Utils.CATEGORY1_POINTS,
            Category2Points = Utils.CATEGORY2_POINTS,
            Category3Points = Utils.CATEGORY3_POINTS,
        };
    }
}