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

    // tests for GetPlayerGameResults

    [Fact]
    public async Task GetPlayerGameResults()
    {
        // arrange
        var bestRankedPlayerGameResult = Utils.CreatePlayerGameResult(rank: 4);
        var notRankedPlayerGameResult = Utils.CreateInitialPlayerGameResult();
        var worstRankedPlayerGameResult = Utils.CreatePlayerGameResult(rank: 10);

        _playerResultRepositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(new List<PlayerGameResult> 
            { 
                notRankedPlayerGameResult,
                worstRankedPlayerGameResult,
                bestRankedPlayerGameResult,
            });

        var expectedPlayerResults = new List<PlayerGameResult>
        {
            bestRankedPlayerGameResult,
            worstRankedPlayerGameResult,
            notRankedPlayerGameResult,
        }.ToImmutableList();

        // act
        var actualPlayerResults = await _service.GetPlayerGameResults();

        // assert
        Assert.Equal(expectedPlayerResults, actualPlayerResults);
        _playerResultRepositoryMock
            .Verify(m => m.GetPlayerGameResults(), Times.Once);
    }

    // tests for CalculatePlayerGameResults

    [Fact]
    public async Task CalculatePlayerGameResults()
    {
        // arrange
        var player1Id = 10;
        var player2Id = 20;

        var player1Result = Utils.CreateInitialPlayerGameResult(player1Id);
        var player2Result = Utils.CreateInitialPlayerGameResult(player2Id);
        var playerResults = 
        _playerResultRepositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(
            [
                player1Result,
                player2Result
            ]
        );

        var player1RatingResult = Utils.CreateRatingGameResult(-5, 0);
        var player2RatingResult = Utils.CreateRatingGameResult(10, -3);
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(player1Id))
            .ReturnsAsync([player1RatingResult]);
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(player2Id))
            .ReturnsAsync([player2RatingResult]);

        // act
        await _service.CalculatePlayerGameResults();

        // assert
        Assert.Equal(5, player1Result.TotalPoints);
        Assert.Equal(7, player2Result.TotalPoints);

        Assert.Equal(1, player1Result.Rank);
        Assert.Equal(2, player2Result.Rank);

        _playerResultRepositoryMock
            .Verify(m => m.GetPlayerGameResults(), Times.Once);

        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(player1Id), Times.Once);
        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(player2Id), Times.Once);

        _playerResultRepositoryMock
            .Verify(m => m.SaveChanges(), Times.Once);
    }

    // tests for CalculateTotalPoints

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
        Assert.Equal(90, playerResult.TotalPoints);

        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(Utils.PLAYER_ID), Times.Once);
    }

    [Fact]
    public async Task CalculateTotalPoints_MissingBonusPoints()
    {
        // arrange
        var playerResult = Utils.CreateInitialPlayerGameResult();

        var ratingResults = new List<RatingGameResult> {
            Utils.CreateRatingGameResult(10, null),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(Utils.PLAYER_ID))
            .ReturnsAsync(ratingResults);

        // act and assert
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
            Utils.CreateRatingGameResult(null, 3),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(Utils.PLAYER_ID))
            .ReturnsAsync(ratingResults);

        // act and assert
        await Assert.ThrowsAsync<Exception>(
            async () => await _service.CalculateTotalPoints(playerResult)
        );
    }

    // tests for CalculateRanks

    [Fact]
    public void CalculateRanks()
    {
        // arrange
        var playerResult1 = Utils.CreatePlayerGameResult(400);
        var playerResult2 = Utils.CreatePlayerGameResult(300);
        var playerResult3 = Utils.CreatePlayerGameResult(200);
        var playerResult4 = Utils.CreatePlayerGameResult(300);
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
}