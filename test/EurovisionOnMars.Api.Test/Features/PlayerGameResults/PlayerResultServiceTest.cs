using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.PlayerGameResults;

public class PlayerResultServiceTest
{
    private readonly Mock<IPlayerGameResultRepository> _playerResultRepositoryMock;
    private readonly Mock<IRatingGameResultService> _ratingGameResultServiceMock;
    private readonly Mock<ILogger<PlayerGameResultService>> _loggerMock;
    private readonly PlayerGameResultService _service;

    private static readonly int PLAYER_ID = 6474373;

    public PlayerResultServiceTest()
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
        var playerResult1 = CreatePlayerResult(1, 4);
        var playerResult2 = CreatePlayerResult(2, null);
        var playerResult3 = CreatePlayerResult(2, 2);

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

        var playerResult1 = CreateInitialPlayerResult(1, playerId1);
        var playerResult2 = CreateInitialPlayerResult(1, playerId2);
        var playerResults = 
        _playerResultRepositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(new List<PlayerGameResult>
            {
                playerResult1,
                playerResult2
            }
        );

        var ratingResults1 = new List<RatingGameResult> { 
            CreateRatingGameResult(100, -5, 0)
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(playerId1))
            .ReturnsAsync(ratingResults1);

        var ratingResults2 = new List<RatingGameResult> {
            CreateRatingGameResult(201, 10, -3)
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(playerId2))
            .ReturnsAsync(ratingResults2);

        // act
        await _service.CalculatePlayerGameResults();

        // assert
        _playerResultRepositoryMock
            .Verify(m => m.GetPlayerGameResults(), Times.Once);

        Assert.Equal(5, playerResult1.TotalPoints);
        Assert.Equal(7, playerResult2.TotalPoints);
        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(playerId1), Times.Once);
        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(playerId2), Times.Once);

        Assert.Equal(2, playerResult1.Rank);
        Assert.Equal(1, playerResult2.Rank);

        _playerResultRepositoryMock
            .Verify(m => m.UpdatePlayerGameResult(playerResult1), Times.Once);
        _playerResultRepositoryMock
            .Verify(m => m.UpdatePlayerGameResult(playerResult2), Times.Once);
    }

    [Fact]
    public async Task CalculateTotalPoints()
    {
        // arrange
        var playerId = 10;
        var playerResult = CreateInitialPlayerResult(1, playerId);

        var ratingResults = new List<RatingGameResult> {
            CreateRatingGameResult(100, -5, 0),
            CreateRatingGameResult(200, 100, -25),
            CreateRatingGameResult(300, 3, 7),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(playerId))
            .ReturnsAsync(ratingResults);

        // act
        await _service.CalculateTotalPoints(playerResult);

        // assert
        Assert.Equal(80, playerResult.TotalPoints);

        _ratingGameResultServiceMock
            .Verify(m => m.GetRatingGameResults(playerId), Times.Once);
    }

    [Fact]
    public async Task CalculateTotalPoints_MissingBonusPoints()
    {
        // arrange
        var playerId = 10;
        var playerResult = CreateInitialPlayerResult(1, playerId);

        var ratingResults = new List<RatingGameResult> {
            CreateRatingGameResult(100, -5, 0),
            CreateRatingGameResult(100, 10, null),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(playerId))
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
        var playerId = 10;
        var playerResult = CreateInitialPlayerResult(1, playerId);

        var ratingResults = new List<RatingGameResult> {
            CreateRatingGameResult(100, -5, 0),
            CreateRatingGameResult(100, null, 3),
        }.ToImmutableList();
        _ratingGameResultServiceMock.Setup(m => m.GetRatingGameResults(playerId))
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
        var playerResult1 = CreatePlayerResult(1, 400);
        var playerResult2 = CreatePlayerResult(2, 300);
        var playerResult3 = CreatePlayerResult(3, 200);
        var playerResult4 = CreatePlayerResult(4, 300);
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

    private PlayerGameResult CreatePlayerResult(int id, int? rank)
    {
        return CreatePlayerGameResult(id, null, rank, PLAYER_ID);
    }

    private PlayerGameResult CreateInitialPlayerResult(int id, int playerId)
    {
        return CreatePlayerGameResult(id, null, null, playerId);
    }

    private PlayerGameResult CreatePlayerResult(int id, int totalPoints)
    {
        return CreatePlayerGameResult(id, totalPoints, null, PLAYER_ID);
    }

    private PlayerGameResult CreatePlayerGameResult(int id, int? totalPoints, int? rank, int playerId)

    {
        return new PlayerGameResult()
        {
            Id = id,
            TotalPoints = totalPoints,
            Rank = rank,
            PlayerId = playerId
        };
    }

    private RatingGameResult CreateRatingGameResult(int id, int? difference, int? bonusPoints)
    {
        return new RatingGameResult()
        {
            Id = id,
            RankDifference = difference,
            BonusPoints = bonusPoints
        };
    }
}