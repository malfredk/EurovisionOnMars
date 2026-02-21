using EurovisionOnMars.Api.Features;
using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.PlayerRatings.Domain;
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
    private readonly Mock<IPlayerRatingProcessor> _playerRatingProcessorMock;

    private readonly PlayerRatingService _service;

    public PlayerRatingServiceTest()
    {
        _repositoryMock = new Mock<IPlayerRatingRepository>();
        _ratingTimeValidatorMock = new Mock<IRatingTimeValidator>();
        _loggerMock = new Mock<ILogger<PlayerRatingService>>();
        _playerRatingProcessorMock = new Mock<IPlayerRatingProcessor>();

        _service = new PlayerRatingService(
            _repositoryMock.Object, 
            _ratingTimeValidatorMock.Object,
            _loggerMock.Object, 
            _playerRatingProcessorMock.Object
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
        var request = Utils.CreateUpdatePlayerRatingRequest();

        var rating = Utils.CreateInitialPlayerRating();
        var otherRating = Utils.CreateInitialPlayerRating(1222);
        var ratings = new List<PlayerRating>() { rating, otherRating };
        _repositoryMock.Setup(m => m.GetPlayerRatingsForPlayer(Utils.RATING_ID))
            .ReturnsAsync(ratings);

        // act
        await _service.UpdatePlayerRating(Utils.RATING_ID, request);

        // assert
        _ratingTimeValidatorMock
            .Verify(m => m.EnsureRatingIsOpen(), Times.Once);

        _repositoryMock
            .Verify(m => m.GetPlayerRatingsForPlayer(Utils.RATING_ID), Times.Once);

        _playerRatingProcessorMock
            .Verify(m => m.UpdatePlayerRating(request, rating, ratings), Times.Once);

        _repositoryMock
            .Verify(m => m.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task UpdatePlayerRating_RatingTimeValidatorThrowsException()
    {
        // arrange
        var request = Utils.CreateUpdatePlayerRatingRequest();

        _ratingTimeValidatorMock.Setup(m => m.EnsureRatingIsOpen())
            .Throws<RatingIsClosedException>();

        // act and assert
        await Assert.ThrowsAsync<RatingIsClosedException>(
            () => _service.UpdatePlayerRating(Utils.RATING_ID, request)
        );

        _ratingTimeValidatorMock.Verify(m => m.EnsureRatingIsOpen(), Times.Once);

        _repositoryMock.VerifyNoOtherCalls();
        _playerRatingProcessorMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePlayerRating_InvalidId()
    {
        // arrange
        var request = Utils.CreateUpdatePlayerRatingRequest();

        var otherRating = Utils.CreateInitialPlayerRating(1222);
        _repositoryMock.Setup(m => m.GetPlayerRatingsForPlayer(Utils.RATING_ID))
            .ReturnsAsync([otherRating]);

        // act and assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdatePlayerRating(Utils.RATING_ID, request)
        );

        _repositoryMock.Verify(m => m.GetPlayerRatingsForPlayer(Utils.RATING_ID), Times.Once);

        _playerRatingProcessorMock.VerifyNoOtherCalls();
        _repositoryMock.VerifyNoOtherCalls();
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
}