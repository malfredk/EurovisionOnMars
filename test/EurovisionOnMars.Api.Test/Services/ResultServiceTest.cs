using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class ResultServiceTest
{
    private readonly Mock<IPlayerRepository> _playerRepositoryMock;
    private readonly Mock<IRatingResultService> _ratingResultServiceMock;
    private readonly Mock<IPlayerResultService> _playerResultServiceMock;
    private readonly Mock<ILogger<ResultService>> _loggerMock;
    private readonly ResultService _service;

    public ResultServiceTest()
    {
        _playerRepositoryMock = new Mock<IPlayerRepository>();
        _ratingResultServiceMock = new Mock<IRatingResultService>();
        _playerResultServiceMock = new Mock<IPlayerResultService>();
        _loggerMock = new Mock<ILogger<ResultService>>();

        _service = new ResultService(
            _playerRepositoryMock.Object,
            _ratingResultServiceMock.Object,
            _playerResultServiceMock.Object,
            _loggerMock.Object
            );
    }

    [Fact]
    public async void CalculateResults()
    {
        // arrange
        var player1Id = 1;
        var player2Id = 2222;


        _playerRepositoryMock.Setup(m => m.GetPlayers())
            .ReturnsAsync(new List<Player> 
            { 
                CreatePlayer(player1Id),
                CreatePlayer(player2Id)
            }.ToImmutableList());

        int previousCallOrder = 0;
        int loop1Call1Order = -1;
        int loop1Call2Order = -1;
        int loop2Call1Order = -1;
        int loop2Call2Order = -1;
        int finalCallOrder = -1;

        _playerResultServiceMock.Setup(m => m.CalculatePlayerRankings())
            .Callback(() => finalCallOrder = previousCallOrder++);

        _ratingResultServiceMock.Setup(m => m.CalculateRatingResults(player1Id))
            .Callback(() => loop1Call1Order = previousCallOrder++ );
        _playerResultServiceMock.Setup(m => m.CalculatePlayerScore(player1Id))
            .Callback(() => loop1Call2Order = previousCallOrder++);

        _ratingResultServiceMock.Setup(m => m.CalculateRatingResults(player2Id))
            .Callback(() => loop2Call1Order = previousCallOrder++ );
        _playerResultServiceMock.Setup(m => m.CalculatePlayerScore(player2Id))
            .Callback(() => loop2Call2Order = previousCallOrder++);

        // act
        await _service.CalculateResults();

        // assert
        _playerRepositoryMock.Verify(m => m.GetPlayers(), Times.Once);

        _ratingResultServiceMock.Verify(m => m.CalculateRatingResults(player1Id), Times.Once);
        Assert.Equal(0, loop1Call1Order);
        _playerResultServiceMock.Verify(m => m.CalculatePlayerScore(player1Id), Times.Once);
        Assert.Equal(1, loop1Call2Order);

        _ratingResultServiceMock.Verify(m => m.CalculateRatingResults(player2Id), Times.Once);
        Assert.Equal(2, loop2Call1Order);
        _playerResultServiceMock.Verify(m => m.CalculatePlayerScore(player2Id), Times.Once);
        Assert.Equal(3, loop2Call2Order);

        _playerResultServiceMock.Verify(m => m.CalculatePlayerRankings(), Times.Once);
        Assert.Equal(4, finalCallOrder);
    }

    private Player CreatePlayer(int id)
    {
        return new Player()
        {
            Id = id,
            Username = "test"
        };
    }
}