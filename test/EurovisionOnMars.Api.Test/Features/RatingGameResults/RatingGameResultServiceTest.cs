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

        var player1Rating1 = Utils.CreateInitialPlayerRating(100, player1Id);
        var player1Rating2 = Utils.CreateInitialPlayerRating(200, player1Id);
        var player2Rating1 = Utils.CreateInitialPlayerRating(300, player2Id);

        var allRatings = new List<PlayerRating>
        {
            player1Rating2,
            player2Rating1,
            player1Rating1
        };
        _playerRatingServiceMock.Setup(m => m.GetAllPlayerRatings())
            .ReturnsAsync(allRatings);

        // act
        await _service.CalculateRatingGameResults();

        // assert
        _playerRatingServiceMock
            .Verify(m => m.GetAllPlayerRatings(), Times.Once);

        Func<IReadOnlyList<PlayerRating>, bool> isPlayer1Group = l =>
            l.Count == 2 && l.Contains(player1Rating1) && l.Contains(player1Rating2);
        Func<IReadOnlyList<PlayerRating>, bool> isPlayer2Group = l =>
            l.Count == 1 && l.Contains(player2Rating1);

        _ratingGameResultCalculatorMock.Verify(m =>
            m.CalculateRatingGameResult(player1Rating1, It.Is<IReadOnlyList<PlayerRating>>(l => isPlayer1Group(l))),
            Times.Once);
        _ratingGameResultCalculatorMock.Verify(m =>
            m.CalculateRatingGameResult(player1Rating2, It.Is<IReadOnlyList<PlayerRating>>(l => isPlayer1Group(l))),
            Times.Once);
        _ratingGameResultCalculatorMock.Verify(m =>
            m.CalculateRatingGameResult(player2Rating1, It.Is<IReadOnlyList<PlayerRating>>(l => isPlayer2Group(l))),
            Times.Once);

        _ratingGameResultRepositoryMock
            .Verify(m => m.SaveChanges(), Times.Once);
    }
}