using EurovisionOnMars.Api.Features.GameResults;
using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Api.Features.RatingGameResults;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.GameResults;

public class GameResultServiceTest
{
    private readonly Mock<IPlayerGameResultService> _playerGameResultServiceMock;
    private readonly Mock<IRatingGameResultService> _ratingGameResultServiceMock;
    private readonly Mock<ILogger<GameResultService>> _loggerMock;
    private readonly GameResultService _service;

    public GameResultServiceTest()
    {
        _playerGameResultServiceMock = new Mock<IPlayerGameResultService>();
        _ratingGameResultServiceMock = new Mock<IRatingGameResultService>();
        _loggerMock = new Mock<ILogger<GameResultService>>();

        _service = new GameResultService(
            _playerGameResultServiceMock.Object,
            _ratingGameResultServiceMock.Object,
            _loggerMock.Object 
        );
    }

    [Fact]
    public async Task CalculateResults()
    {
        // arrange
        var sequence = new MockSequence();

        _ratingGameResultServiceMock
            .InSequence(sequence)
            .Setup(s => s.CalculateRatingGameResults())
            .Returns(Task.CompletedTask);
        _playerGameResultServiceMock
            .InSequence(sequence)
            .Setup(s => s.CalculatePlayerGameResults())
            .Returns(Task.CompletedTask);

        // act
        await _service.CalculateGameResults();

        // assert
        _ratingGameResultServiceMock.Verify(m => m.CalculateRatingGameResults(), Times.Once);
        _playerGameResultServiceMock.Verify(m => m.CalculatePlayerGameResults(), Times.Once);
    }
}
