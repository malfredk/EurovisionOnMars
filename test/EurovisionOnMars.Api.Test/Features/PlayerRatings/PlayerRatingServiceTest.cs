using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.RatingClosing;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings;

public class PlayerRatingServiceTest
{
    private readonly Mock<IPlayerRatingRepository> _repositoryMock;
    private readonly Mock<IRatingClosingService> _ratingClosingServiceMock;
    private readonly Mock<ILogger<PlayerRatingService>> _loggerMock;
    private readonly Mock<ISpecialPointsValidator> _specialPointsValidatorMock;
    private readonly Mock<IRankHandler> _rankHandlerMock;
    private readonly PlayerRatingService _service;

    public PlayerRatingServiceTest()
    {
        _repositoryMock = new Mock<IPlayerRatingRepository>();
        _ratingClosingServiceMock = new Mock<IRatingClosingService>();
        _loggerMock = new Mock<ILogger<PlayerRatingService>>();
        _specialPointsValidatorMock = new Mock<ISpecialPointsValidator>();
        _rankHandlerMock = new Mock<IRankHandler>();

        _service = new PlayerRatingService(
            _repositoryMock.Object, 
            _ratingClosingServiceMock.Object,
            _loggerMock.Object, 
            _specialPointsValidatorMock.Object,
            _rankHandlerMock.Object
            );
    }

    // tests for getting all ratings

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
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
        _repositoryMock.Verify(r => r.GetAllPlayerRatings(), Times.Once());
    }

    // tests for getting ratings by player id

    [Fact]
    public async Task GetRatingsByPlayerId()
    {
        // arrange
        var rating1 = CreatePlayerRating(1, 26);
        var rating2 = CreatePlayerRating(10, null);
        var rating3 = CreatePlayerRating(8, null);
        var rating4 = CreatePlayerRating(26, 1);
        ImmutableList<PlayerRating> ratings = [rating1, rating2, rating3, rating4];
        ImmutableList<PlayerRating> expectedRatings = [rating4, rating1, rating3, rating2];

        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID))
            .ReturnsAsync(ratings);

        // act
        var actualRatings = await _service.GetPlayerRatingsByPlayerId(Utils.PLAYER_ID);

        // assert
        Assert.Equal(expectedRatings, actualRatings);

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
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

    // TODO: tests for updating points in player rating

    // TODO: tests for updating rank in player rating

    private static PlayerRating CreatePlayerRating(int countryNumber, int? predictionRank)
    {
        var country = Utils.CreateInitialCountry(countryNumber);
        var player = Utils.CreateInitialPlayerWithOneCountry();

        var rating = new PlayerRating(player, country);
        rating.Prediction.SetCalculatedRank(predictionRank);
        return rating;
    }
}