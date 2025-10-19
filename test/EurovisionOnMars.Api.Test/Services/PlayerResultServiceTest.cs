using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class PlayerResultServiceTest
{
    private readonly Mock<IRatingResultRepository> _ratingResultRepositoryMock;
    private readonly Mock<IPlayerResultRepository> _playerResultRepositoryMock;
    private readonly Mock<ILogger<PlayerResultService>> _loggerMock;
    private readonly PlayerResultService _service;

    public PlayerResultServiceTest()
    {
        _ratingResultRepositoryMock = new Mock<IRatingResultRepository>();
        _playerResultRepositoryMock = new Mock<IPlayerResultRepository>();
        _loggerMock = new Mock<ILogger<PlayerResultService>>();

        _service = new PlayerResultService(
            _ratingResultRepositoryMock.Object,
            _playerResultRepositoryMock.Object,
            _loggerMock.Object
            );
    }

    [Fact]
    public async void CalculatePlayerScore()
    {
        // arrange
        var playerId = 34;
        var initialPlayerResult = CreatePlayerResult(900, null);

        var ratingResult1 = CreateRatingResult(-10, 0);
        var ratingResult2 = CreateRatingResult(3, 0);
        var ratingResult3 = CreateRatingResult(0, -5);

        var expectedScore = 8;
        var expectedPlayerResult = CreatePlayerResult(900, expectedScore);
        var expectedReturnedPlayerResult = CreatePlayerResult(56, 77);

        _playerResultRepositoryMock.Setup(m => m.GetPlayerResult(playerId))
            .ReturnsAsync(initialPlayerResult);
        _ratingResultRepositoryMock.Setup(m => m.GetRatingResultsForPlayer(playerId))
            .ReturnsAsync(new List<RatingGameResult> { ratingResult1, ratingResult2, ratingResult3 }.ToImmutableList());
        _playerResultRepositoryMock.Setup(m => m.UpdatePlayerResult(expectedPlayerResult))
            .ReturnsAsync(expectedReturnedPlayerResult);

        // act
        var actualPlayerResult = await _service.CalculatePlayerScore(playerId);

        // assert
        Assert.Equal(expectedReturnedPlayerResult, actualPlayerResult);

        _playerResultRepositoryMock.Verify(m => m.GetPlayerResult(playerId), Times.Once);
        _ratingResultRepositoryMock.Verify(m => m.GetRatingResultsForPlayer(playerId), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(expectedPlayerResult), Times.Once);
    }

    [Theory]
    [InlineData(null, 0)]
    [InlineData(4, null)]
    public async void CalculatePlayerScore_Invalid(int? difference, int? bonusPoints)
    {
        // arrange
        var playerId = 34;
        var initialPlayerResult = CreatePlayerResult(900, null);

        var ratingResult1 = CreateRatingResult(-10, 0);
        var ratingResult2 = CreateRatingResult(difference, bonusPoints);
        var ratingResult3 = CreateRatingResult(0, -5);

        _playerResultRepositoryMock.Setup(m => m.GetPlayerResult(playerId))
            .ReturnsAsync(initialPlayerResult);
        _ratingResultRepositoryMock.Setup(m => m.GetRatingResultsForPlayer(playerId))
            .ReturnsAsync(new List<RatingGameResult> { ratingResult1, ratingResult2, ratingResult3 }.ToImmutableList());

        // act and assert
        await Assert.ThrowsAsync<Exception>(async () => await _service.CalculatePlayerScore(playerId));

        _playerResultRepositoryMock.Verify(m => m.GetPlayerResult(playerId), Times.Once);
        _ratingResultRepositoryMock.Verify(m => m.GetRatingResultsForPlayer(playerId), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(It.IsAny<PlayerGameResult>()), Times.Never);
    }

    [Fact]
    public async void CalculatePlayerRanks()
    {
        // arrange
        var initialPlayerResult1 = CreatePlayerResult(1, 100);
        var initialPlayerResult2 = CreatePlayerResult(2, -50);
        var initialPlayerResult3 = CreatePlayerResult(3, 10);
        var initialPlayerResult4 = CreatePlayerResult(4, 0);
        var initialPlayerResult5 = CreatePlayerResult(5, 10);

        var expectedPlayerResult1 = CreatePlayerResult(1, 100, 5);
        var expectedPlayerResult2 = CreatePlayerResult(2, -50, 1);
        var expectedPlayerResult3 = CreatePlayerResult(3, 10, 3);
        var expectedPlayerResult4 = CreatePlayerResult(4, 0, 2);
        var expectedPlayerResult5 = CreatePlayerResult(5, 10, 3);

        _playerResultRepositoryMock.Setup(m => m.GetPlayerResults())
            .ReturnsAsync(new List<PlayerGameResult> 
            { 
                initialPlayerResult1,
                initialPlayerResult2,
                initialPlayerResult3,
                initialPlayerResult4,
                initialPlayerResult5
            }.ToImmutableList());

        // act
        await _service.CalculatePlayerRanks();

        // assert
        _playerResultRepositoryMock.Verify(m => m.GetPlayerResults(), Times.Once);

        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(expectedPlayerResult1), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(expectedPlayerResult2), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(expectedPlayerResult3), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(expectedPlayerResult4), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(expectedPlayerResult5), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(It.IsAny<PlayerGameResult>()), Times.Exactly(5));
    }

    [Fact]
    public async void CalculatePlayerRanks_Invalid()
    {
        // arrange
        var initialPlayerResult1 = CreatePlayerResult(1, 100);
        var initialPlayerResult2 = CreatePlayerResult(2, null);

        _playerResultRepositoryMock.Setup(m => m.GetPlayerResults())
            .ReturnsAsync(new List<PlayerGameResult>
            {
                initialPlayerResult1,
                initialPlayerResult2
            }.ToImmutableList());

        // act and assert
        await Assert.ThrowsAsync<Exception>(async () => await _service.CalculatePlayerRanks());

        _playerResultRepositoryMock.Verify(m => m.GetPlayerResults(), Times.Once);
        _playerResultRepositoryMock.Verify(m => m.UpdatePlayerResult(It.IsAny<PlayerGameResult>()), Times.Never);
    }

    private PlayerGameResult CreatePlayerResult(int id, int? score)
    {
        return CreatePlayerResult(id, score, null);
    }

    private PlayerGameResult CreatePlayerResult(int id, int? score, int? rank)
    {
        return new PlayerGameResult()
        {
            Id = id,
            TotalPoints = score,
            Rank = rank
        };
    }

    private RatingGameResult CreateRatingResult(int? difference, int? bonusPoints)
    {
        return new RatingGameResult()
        {
            RankDifference = difference,
            BonusPoints = bonusPoints
        };
    }
}