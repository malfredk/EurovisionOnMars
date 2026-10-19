using EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;
using EurovisionOnMars.TestSupport.Game;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.GameResults.CalculateGameResults;

public class CalculateGameResultsServiceTest
{
    private readonly Mock<ILogger<CalculateGameResultsService>> _loggerMock;
    private readonly Mock<ICalculateGameResultsRepository> _repositoryMock;
    
    private readonly CalculateGameResultsService _service;

    public CalculateGameResultsServiceTest()
    {
        _loggerMock = new Mock<ILogger<CalculateGameResultsService>>();
        _repositoryMock = new Mock<ICalculateGameResultsRepository>();

        _service = new CalculateGameResultsService(
            _loggerMock.Object,
            _repositoryMock.Object
            );
    }

    [Fact]
    public async Task CalculateGameResults_GetsAndSavesPlayers()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGame(playersCount: 2, countriesCount: 2);
        game.SetActualRanksForCountries();

        var players = game.Players;

        _repositoryMock.Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

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
        var game = GameScenarioFactory.CreateInitialGame(playersCount: 2, countriesCount: 2);
        game.SetActualRanksForCountries();

        var players = game.Players;

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

    [Fact]
    public async Task CalculateGameResults_CalculationFails_ThrowsAndDoesNotSave()
    {
        // arrange
        var game = GameScenarioFactory.CreateInitialGame(playersCount: 1, countriesCount: 1);
        var players = game.Players;

        _repositoryMock.Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CalculateGameResults());

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }
}