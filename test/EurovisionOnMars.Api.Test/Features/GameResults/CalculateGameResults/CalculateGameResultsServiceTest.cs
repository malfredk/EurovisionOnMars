using EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;
using EurovisionOnMars.Entity.Game;
using EurovisionOnMars.Entity.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.GameResults.CalculateGameResults;

public class CalculateGameResultsServiceTest
{
    private readonly Mock<ILogger<CalculateGameResultsService>> _loggerMock;
    private readonly Mock<ICalculateGameResultsRepository> _repositoryMock;
    private readonly PlayerRanksCalculator _playerRanksCalculator;
    
    private readonly CalculateGameResultsService _service;

    public CalculateGameResultsServiceTest()
    {
        _loggerMock = new Mock<ILogger<CalculateGameResultsService>>();
        _repositoryMock = new Mock<ICalculateGameResultsRepository>();
        _playerRanksCalculator = new PlayerRanksCalculator();

        _service = new CalculateGameResultsService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _playerRanksCalculator
            );
    }

    [Fact]
    public async Task CalculateGameResults_GetsPlayers()
    {
        // arrange
        IReadOnlyList<Player> players = [];

        _repositoryMock
            .Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        _repositoryMock
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        // act
        await _service.CalculateGameResults();

        // assert
        _repositoryMock.Verify(
            r => r.GetPlayers(),
            Times.Once);
    }

    [Fact]
    public async Task CalculateGameResults_CalculatesPlayerRanks()
    {
        // arrange
        IReadOnlyList<Player> players = [];

        _repositoryMock
            .Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        _repositoryMock
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

        _repositoryMock
            .Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        _repositoryMock
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        // act
        await _service.CalculateGameResults();

        // assert
        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }
}