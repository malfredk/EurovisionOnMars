using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.PlayerGameResults;

public class PlayerGameResultServiceTest
{
    private readonly Mock<IPlayerGameResultRepository> _playerResultRepositoryMock;
    private readonly Mock<IRatingGameResultService> _ratingGameResultServiceMock;
    private readonly Mock<ILogger<PlayerGameResultService>> _loggerMock;
    private readonly PlayerGameResultService _service;

    public PlayerGameResultServiceTest()
    {
        _playerResultRepositoryMock = new Mock<IPlayerGameResultRepository>();
        _ratingGameResultServiceMock = new Mock<IRatingGameResultService>();
        _loggerMock = new Mock<ILogger<PlayerGameResultService>>();

        _service = new PlayerGameResultService(
            _playerResultRepositoryMock.Object,
            _loggerMock.Object,
            _ratingGameResultServiceMock.Object
            );
    }

    [Fact]
    public async Task GetPlayerGameResults()
    {
        // arrange
        var playerResult1 = CreatePlayerGameResultWithRank(4);
        var playerResult2 = CreatePlayerGameResultWithRank(null);
        var playerResult3 = CreatePlayerGameResultWithRank(2);

        var expectedPlayerResults = new List<PlayerGameResult>
        {
            playerResult3,
            playerResult1,
            playerResult2,
        }.ToImmutableList();

        _playerResultRepositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(new List<PlayerGameResult> 
            { 
                playerResult1,
                playerResult2,
                playerResult3,
            });

        // act
        var actualPlayerResults = await _service.GetPlayerGameResults();

        // assert
        Assert.Equal(expectedPlayerResults, actualPlayerResults);
        _playerResultRepositoryMock
            .Verify(m => m.GetPlayerGameResults(), Times.Once);
    }

    [Fact]
    public async Task CalculatePlayerGameResults()
    {
        // arrange
        var playerId1 = 10;
        var playerId2 = 20;

        var playerResult1 = Utils.CreateInitialPlayerGameResult(playerId1);
        var playerResult2 = Utils.CreateInitialPlayerGameResult(playerId2);
        var playerResults = 
        _playerResultRepositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(
            [
                playerResult1,
                playerResult2
            ]
        );

        var ratingResult1 = Utils.CreateRatingGameResult(-5, 0);
        var ratingResult2 = Utils.CreateRatingGameResult(10, -3);

        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(playerId1))
            .ReturnsAsync([ratingResult1]);
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(playerId2))
            .ReturnsAsync([ratingResult2]);

        // act
        await _service.CalculatePlayerGameResults();

        // assert
        _playerResultRepositoryMock
            .Verify(m => m.GetPlayerGameResults(), Times.Once);

        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(playerId1), Times.Once);
        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(playerId2), Times.Once);

        _playerResultRepositoryMock
            .Verify(m => m.UpdatePlayerGameResult(playerResult1), Times.Once);
        _playerResultRepositoryMock
            .Verify(m => m.UpdatePlayerGameResult(playerResult2), Times.Once);
    }

    [Fact]
    public async Task CalculateTotalPoints()
    {
        // arrange
        var playerResult = Utils.CreateInitialPlayerGameResult();

        var ratingResults = new List<RatingGameResult> {
            Utils.CreateRatingGameResult(-5, 0),
            Utils.CreateRatingGameResult(100, -25),
            Utils.CreateRatingGameResult(3, 7),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(Utils.PLAYER_ID))
            .ReturnsAsync(ratingResults);

        // act
        await _service.CalculateTotalPoints(playerResult);

        // assert
        Assert.Equal(80, playerResult.TotalPoints);

        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(Utils.PLAYER_ID), Times.Once);
    }

    [Fact]
    public async Task CalculateTotalPoints_MissingBonusPoints()
    {
        // arrange
        var playerResult = Utils.CreateInitialPlayerGameResult();

        var ratingResults = new List<RatingGameResult> {
            Utils.CreateRatingGameResult(-5, 0),
            Utils.CreateRatingGameResult(10, null),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(Utils.PLAYER_ID))
            .ReturnsAsync(ratingResults);

        // act and assert
        await _service.CalculateTotalPoints(playerResult);

        // assert
        await Assert.ThrowsAsync<Exception>(
            async () => await _service.CalculateTotalPoints(playerResult)
        );
    }

    [Fact]
    public async Task CalculateTotalPoints_MissingRankDifference()
    {
        // arrange
        var playerResult = Utils.CreateInitialPlayerGameResult();

        var ratingResults = new List<RatingGameResult> {
            Utils.CreateRatingGameResult(-5, 0),
            Utils.CreateRatingGameResult(null, 3),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(Utils.PLAYER_ID))
            .ReturnsAsync(ratingResults);

        // act and assert
        await _service.CalculateTotalPoints(playerResult);

        // assert
        await Assert.ThrowsAsync<Exception>(
            async () => await _service.CalculateTotalPoints(playerResult)
        );
    }

    [Fact]
    public void CalculateRanks()
    {
        // arrange
        var playerResult1 = CreatePlayerGameResultWithPoints(400);
        var playerResult2 = CreatePlayerGameResultWithPoints(300);
        var playerResult3 = CreatePlayerGameResultWithPoints(200);
        var playerResult4 = CreatePlayerGameResultWithPoints(300);
        var playerResults = new List<PlayerGameResult>
        {
            playerResult1,
            playerResult2,
            playerResult3,
            playerResult4
        };

        // act
        _service.CalculateRanks(playerResults);

        // assert
        Assert.Equal(playerResult1.Rank, 4);
        Assert.Equal(playerResult2.Rank, 2);
        Assert.Equal(playerResult3.Rank, 1);
        Assert.Equal(playerResult4.Rank, 2);
    }

    private PlayerGameResult CreatePlayerGameResultWithRank(int? rank)
    {
        return Utils.CreatePlayerGameResult(rank, null);
    }

    private PlayerGameResult CreatePlayerGameResultWithPoints(int totalPoints)
    {
        return Utils.CreatePlayerGameResult(null, totalPoints);
    }
}