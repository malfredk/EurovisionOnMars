using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Api.Test.Features;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class RatingGameResultServiceTest
{
    private readonly Mock<IRatingGameResultRepository> _ratingGameResultRepositoryMock;
    private readonly Mock<IPlayerRatingService> _playerRatingServiceMock;
    private readonly Mock<IRatingGameResultCalculator> _ratingGameResultCalculatorMock;
    private readonly Mock<ILogger<RatingGameResultService>> _loggerMock;
    private readonly RatingGameResultService _service;

    public RatingGameResultServiceTest()
    {
        _ratingGameResultRepositoryMock = new Mock<IRatingGameResultRepository>();
        _playerRatingServiceMock = new Mock<IPlayerRatingService>();
        _ratingGameResultCalculatorMock = new Mock<IRatingGameResultCalculator>();
        _loggerMock = new Mock<ILogger<RatingGameResultService>>();

        _service = new RatingGameResultService(
            _ratingGameResultRepositoryMock.Object,
            _playerRatingServiceMock.Object,
            _ratingGameResultCalculatorMock.Object,
            _loggerMock.Object);
    }

    // tests for GetRatingGameResults

    [Fact]
    public async Task GetRatingGameResults()
    {
        // arrange
        var ratingGameResult1 = Utils.CreateInitialRatingGameResult();
        var ratingGameResult2 = Utils.CreateInitialRatingGameResult();
        var ratingGameResults = new List<RatingGameResult>
        {
            ratingGameResult1,
            ratingGameResult2
        }.ToImmutableList();

        _ratingGameResultRepositoryMock.Setup(m => m.GetRatingGameResults(Utils.PLAYER_ID))
            .ReturnsAsync(ratingGameResults);

        // act
        var actualResults = await _service.GetRatingGameResults(Utils.PLAYER_ID);

        // assert
        Assert.Equal(ratingGameResults, actualResults);

        _ratingGameResultRepositoryMock
            .Verify(m => m.GetRatingGameResults(Utils.PLAYER_ID), Times.Once);
    }

    // tests for CalculateRatingGameResults

    [Fact]
    public async Task CalculateRatingGameResults()
    {
        // arrange
        var player1Id = 10;
        var player2Id = 20;

        var rating1Player1 = Utils.CreatePlayerRating(100, player1Id);
        var rating2Player1 = Utils.CreatePlayerRating(200, player1Id);
        var rating3Player2 = Utils.CreatePlayerRating(300, player2Id);

        var ratingsPlayer1 = new List<PlayerRating> 
        {
            rating1Player1,
            rating2Player1
        };
        var ratingsPlayer2 = new List<PlayerRating> { rating3Player2 };

        var allRatings = new List<PlayerRating>
        {
            rating2Player1,
            rating3Player2,
            rating1Player1
        };
        _playerRatingServiceMock.Setup(m => m.GetAllPlayerRatings())
            .ReturnsAsync(allRatings);

        // act
        await _service.CalculateRatingGameResults();

        // assert
        _playerRatingServiceMock
            .Verify(m => m.GetAllPlayerRatings(), Times.Once);

        _ratingGameResultCalculatorMock
            .Verify(m => m.CalculateRatingGameResult(rating1Player1, ratingsPlayer1), Times.Once);
        _ratingGameResultCalculatorMock
            .Verify(m => m.CalculateRatingGameResult(rating2Player1, ratingsPlayer1), Times.Once);
        _ratingGameResultCalculatorMock
            .Verify(m => m.CalculateRatingGameResult(rating3Player2, ratingsPlayer2), Times.Once);
        _ratingGameResultCalculatorMock.VerifyNoOtherCalls();

        _ratingGameResultRepositoryMock
            .Verify(m => m.SaveChanges(), Times.Once);
    }
}