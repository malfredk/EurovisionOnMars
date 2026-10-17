using EurovisionOnMars.Api.Features.Players.GetPlayer;
using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.TestSupport.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Players.GetPlayer;

public class GetPlayerServiceTest
{
    private readonly Mock<ILogger<GetPlayerService>> _loggerMock;
    private readonly Mock<IGetPlayerRepository> _repositoryMock;

    private readonly GetPlayerService _service;

    public GetPlayerServiceTest()
    {
        _loggerMock = new Mock<ILogger<GetPlayerService>>();
        _repositoryMock = new Mock<IGetPlayerRepository>();

        _service = new GetPlayerService(
            _loggerMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task GetPlayer_PlayerExists_ReturnsDto()
    {
        // arrange
        var username = PlayerTestData.Username;
        var expectedPlayerId = 1234;

        _repositoryMock
            .Setup(r => r.GetPlayer(
                It.Is<Username>(u => u.Value == username)))
            .ReturnsAsync(expectedPlayerId);

        // act
        var actualPlayerId = await _service.GetPlayer(username);

        // assert
        Assert.Equal(expectedPlayerId, actualPlayerId);

        _repositoryMock.Verify(
            r => r.GetPlayer(
                It.Is<Username>(u => u.Value == username)),
            Times.Once);
    }

    [Fact]
    public async Task GetPlayer_PlayerDoesNotExist_ThrowsKeyNotFoundException()
    {
        // arrange
        var username = PlayerTestData.Username;

        _repositoryMock
            .Setup(r => r.GetPlayer(It.IsAny<Username>()))
            .ReturnsAsync((int?)null);

        // act
        var action = () => _service.GetPlayer(username);

        // assert
        await Assert.ThrowsAsync<KeyNotFoundException>(action);
    }

    [Fact]
    public async Task GetPlayer_InvalidUsername_DoesNotAccessRepository()
    {
        // arrange
        const string invalidUsername = "invalid username";

        // act
        var action = () => _service.GetPlayer(invalidUsername);

        // assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        _repositoryMock.Verify(
            r => r.GetPlayer(It.IsAny<Username>()),
            Times.Never);
    }
}