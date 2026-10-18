using EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;
using EurovisionOnMars.Domain.Players.GameResults;
using EurovisionOnMars.Dto.GameResults.GetPlayerResults;
using EurovisionOnMars.TestSupport.Game;
using EurovisionOnMars.TestSupport.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.GameResults.GetPlayerGameResults;

public class GetPlayerGameResultsServiceTest
{
    private readonly Mock<ILogger<GetPlayerGameResultsService>> _loggerMock;
    private readonly Mock<IGetPlayerGameResultsRepository> _repositoryMock;
    private readonly Mock<IPlayerGameResultMapper> _mapperMock;
   
    private readonly GetPlayerGameResultsService _service;

    public GetPlayerGameResultsServiceTest()
    {
        _loggerMock = new Mock<ILogger<GetPlayerGameResultsService>>();
        _repositoryMock = new Mock<IGetPlayerGameResultsRepository>();
        _mapperMock = new Mock<IPlayerGameResultMapper>();

        _service = new GetPlayerGameResultsService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object
            );
    }

    [Fact]
    public async Task GetPlayerGameResults_WithoutGameRanks_CallsRepository()
    {
        // arrange
        var gameScenario = GameScenarioFactory.CreateInitialGame(playersCount: 2, countriesCount: 1);
        var playersGameResults = gameScenario.Players
            .Select(p => p.PlayerGameResult)
            .ToList();

        _repositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(playersGameResults);

        _mapperMock.Setup(m => m.ToDto(It.IsAny<PlayerGameResult>()))
            .Returns<PlayerGameResult>(pgr => CreatePlayerGameResultDto());

        // act
        await _service.GetPlayerGameResults();

        // assert
        _repositoryMock
            .Verify(m => m.GetPlayerGameResults(), Times.Once);
    }

    [Fact]
    public async Task GetPlayerGameResults_WithoutGameRanks_ReturnsDtos()
    {
        // arrange
        var gameScenario = GameScenarioFactory.CreateInitialGame(playersCount: 2, countriesCount: 1);
        var playersGameResults = gameScenario.Players
            .Select(p => p.PlayerGameResult)
            .ToList();

        _repositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(playersGameResults);

        var playerGameResultDto1 = CreatePlayerGameResultDto();
        var playerGameResultDto2 = CreatePlayerGameResultDto();
        var expectedPlayerResults = new List<PlayerGameResultDto>
        {
            playerGameResultDto1,
            playerGameResultDto2
        };

        _mapperMock.Setup(m => m.ToDto(playersGameResults[0]))
            .Returns(playerGameResultDto1);
        _mapperMock.Setup(m => m.ToDto(playersGameResults[1]))
            .Returns(playerGameResultDto2);

        // act
        var actualResults = await _service.GetPlayerGameResults();

        // assert
        Assert.All(
            expectedPlayerResults,
            expected => Assert.Contains(expected, actualResults));
    }

    [Fact]
    public async Task GetPlayerGameResults_WithRanks_ReturnsSortedDtos()
    {
        // arrange
        var gameScenario = GameScenarioFactory.CreateInitialGame(playersCount: 3, countriesCount: 1);
        var playersGameResults = gameScenario.Players
            .Select(p => p.PlayerGameResult)
            .ToList();

        _repositoryMock.Setup(m => m.GetPlayerGameResults())
            .ReturnsAsync(playersGameResults);

        var playerGameResultDto1 = CreatePlayerGameResultDto(1);
        var playerGameResultDto2 = CreatePlayerGameResultDto(2);
        var playerGameResultDto3 = CreatePlayerGameResultDto(3);
        var expectedSortedPlayerResults = new List<PlayerGameResultDto>
        {
            playerGameResultDto1,
            playerGameResultDto2,
            playerGameResultDto3
        };

        _mapperMock.Setup(m => m.ToDto(playersGameResults[0]))
            .Returns(playerGameResultDto2);
        _mapperMock.Setup(m => m.ToDto(playersGameResults[1]))
            .Returns(playerGameResultDto3);
        _mapperMock.Setup(m => m.ToDto(playersGameResults[2]))
            .Returns(playerGameResultDto1);

        // act
        var actualPlayerResults = await _service.GetPlayerGameResults();

        // assert
        Assert.Equal(expectedSortedPlayerResults, actualPlayerResults);
    }

    private static PlayerGameResultDto CreatePlayerGameResultDto(int? rank = null)
    {
        return new PlayerGameResultDto
        {
            PlayerUsername = PlayerTestData.Username,
            Rank = rank,
            TotalPoints = 100
        };
    }
}