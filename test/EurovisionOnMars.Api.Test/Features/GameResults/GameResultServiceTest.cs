using EurovisionOnMars.Api.Features.GameResults;
using EurovisionOnMars.Entity.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.GameResults;

public class GameResultServiceTest // TODO
{
    private readonly Mock<IPlayerRanksCalculator> _playerRanksCalculatorMock;
    private readonly Mock<IGameResultRepository> _gameResultRepositoryMock;
    private readonly Mock<ILogger<GameResultService>> _loggerMock;
    private readonly GameResultService _service;

    public GameResultServiceTest()
    {
        _playerRanksCalculatorMock = new Mock<IPlayerRanksCalculator>();
        _gameResultRepositoryMock = new Mock<IGameResultRepository>();
        _loggerMock = new Mock<ILogger<GameResultService>>();

        _service = new GameResultService(
            _playerRanksCalculatorMock.Object,
            _gameResultRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CalculateGameResults_GetsPlayers()
    {
        // arrange
        IReadOnlyList<Player> players = [];

        _gameResultRepositoryMock
            .Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        _gameResultRepositoryMock
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        // act
        await _service.CalculateGameResults();

        // assert
        _gameResultRepositoryMock.Verify(
            r => r.GetPlayers(),
            Times.Once);
    }

    [Fact]
    public async Task CalculateGameResults_CalculatesPlayerRanks()
    {
        // arrange
        IReadOnlyList<Player> players = [];

        _gameResultRepositoryMock
            .Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        _gameResultRepositoryMock
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        // act
        await _service.CalculateGameResults();

        // assert
        _playerRanksCalculatorMock.Verify(
            c => c.CalculatePlayerRanks(players),
            Times.Once);
    }

    [Fact]
    public async Task CalculateGameResults_SavesChanges()
    {
        // arrange
        IReadOnlyList<Player> players = [];

        _gameResultRepositoryMock
            .Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        _gameResultRepositoryMock
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        // act
        await _service.CalculateGameResults();

        // assert
        _gameResultRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }
}