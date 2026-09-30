using EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;
using EurovisionOnMars.Api.Test.TestData.Game;
using EurovisionOnMars.Entity.Game;
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
    public async Task CalculateGameResults_RepositoryIsCalled()
    {
        // arrange
        var gameScenario = GameScenarioFactory.CreateGameWhereActualCountryRanksAreSet();
        _repositoryMock.Setup(r => r.GetPlayers())
            .ReturnsAsync(gameScenario.Players);

        // act
        await _service.CalculateGameResults();

        // assert
        _repositoryMock.Verify(
            r => r.GetPlayers(),
            Times.Once);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    [Fact]
    public async Task CalculateGameResults_GameResultsAreCalculated()
    {
        // arrange
        var gameScenario = GameScenarioFactory.CreateGameWhereActualCountryRanksAreSet();
        var players = gameScenario.Players;

        _repositoryMock.Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        // act
        await _service.CalculateGameResults();

        // assert
        foreach (var player in players)
        {
            foreach (var rating in player.PlayerRatings)
            {
                var ratingGameResult = rating.RatingGameResult;
                Assert.NotNull(ratingGameResult.RankDifference);
                Assert.NotNull(ratingGameResult.BonusPoints);
            }
            var playerGameResult = player.PlayerGameResult;
            Assert.NotNull(playerGameResult.TotalPoints);
            Assert.NotNull(playerGameResult.Rank);
        }
    }
}