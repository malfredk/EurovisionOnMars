using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.Api.Features.Players.RateCountry;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Dto.Players.RateCountry;
using EurovisionOnMars.TestSupport.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Players.RateCountry;

public class RateCountryServiceTest
{
    private const int PlayerId = 123;

    private readonly Mock<ILogger<RateCountryService>> _loggerMock;
    private readonly Mock<IRateCountryRepository> _repositoryMock;
    private readonly Mock<IRatingTimeValidator> _ratingTimeValidatorMock;

    private readonly RateCountryService _service;

    public RateCountryServiceTest()
    {
        _loggerMock = new Mock<ILogger<RateCountryService>>();
        _repositoryMock = new Mock<IRateCountryRepository>();
        _ratingTimeValidatorMock = new Mock<IRatingTimeValidator>();

        _service = new RateCountryService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _ratingTimeValidatorMock.Object);
    }

    [Fact]
    public async Task RateCountry_ValidRequest_RatesCountryAndSavesChanges()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        var rating = player.PlayerRatings[0];
        var ratingId = rating.Id;

        var request = CreateRequest();

        _repositoryMock
            .Setup(r => r.GetPlayer(PlayerId))
            .ReturnsAsync(player);

        // act
        await _service.RateCountry(
            PlayerId,
            ratingId,
            request);

        // assert
        Assert.Equal(
            PlayerRatingTestData.Category1Points,
            rating.Category1Points!.Value);
        Assert.Equal(
            PlayerRatingTestData.Category2Points,
            rating.Category2Points!.Value);
        Assert.Equal(
            PlayerRatingTestData.Category3Points,
            rating.Category3Points!.Value);

        _repositoryMock.Verify(
            r => r.GetPlayer(PlayerId),
            Times.Once);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    [Fact]
    public async Task RateCountry_ValidRequest_VerifiesRatingIsOpen()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        var rating = player.PlayerRatings[0];
        var ratingId = rating.Id;

        var request = CreateRequest();

        _repositoryMock
            .Setup(r => r.GetPlayer(PlayerId))
            .ReturnsAsync(player);

        // act
        await _service.RateCountry(
            PlayerId,
            ratingId,
            request);

        // assert
        _ratingTimeValidatorMock.Verify(
            v => v.EnsureRatingIsOpen(),
            Times.Once);
    }

    [Fact]
    public async Task RateCountry_PlayerDoesNotExist_ThrowsArgumentException()
    {
        // arrange
        var request = CreateRequest();

        _repositoryMock
            .Setup(r => r.GetPlayer(PlayerId))
            .ReturnsAsync((Player?)null);

        // act
        var action = () => _service.RateCountry(
            PlayerId,
            567,
            request);

        // assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task RateCountry_RatingIsClosed_DoesNotGetPlayerNorSaveChanges()
    {
        // arrange
        var request = CreateRequest();

        _ratingTimeValidatorMock
            .Setup(v => v.EnsureRatingIsOpen())
            .Throws<Exception>();

        // act
        var action = () => _service.RateCountry(
            PlayerId,
            567,
            request);

        // assert
        await Assert.ThrowsAsync<Exception>(action);

        _repositoryMock.Verify(
            r => r.GetPlayer(It.IsAny<int>()),
            Times.Never);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task RateCountry_InvalidCategoryPoints_DoesRateCountryNorSaveChanges()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        var rating = player.PlayerRatings[0];
        var ratingId = rating.Id;

        var invalidRequest = CreateRequest(0);

        _repositoryMock
            .Setup(r => r.GetPlayer(PlayerId))
            .ReturnsAsync(player);

        // act
        var action = () => _service.RateCountry(
            PlayerId,
            ratingId,
            invalidRequest);

        // assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        Assert.Null(rating.Category1Points);
        Assert.Null(rating.Category2Points);
        Assert.Null(rating.Category3Points);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task RateCountry_InvalidRatingId_DoesSaveChanges()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        var invalidRatingId = 567;

        var invalidRequest = CreateRequest(0);

        _repositoryMock
            .Setup(r => r.GetPlayer(PlayerId))
            .ReturnsAsync(player);

        // act
        var action = () => _service.RateCountry(
            PlayerId,
            invalidRatingId,
            invalidRequest);

        // assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    private static RateCountryRequestDto CreateRequest(int category1Points = PlayerRatingTestData.Category1Points)
    {
        return new RateCountryRequestDto
        {
            Category1Points = category1Points,
            Category2Points = PlayerRatingTestData.Category2Points,
            Category3Points = PlayerRatingTestData.Category3Points
        };
    }
}