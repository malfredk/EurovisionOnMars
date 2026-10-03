using EurovisionOnMars.Api.Features.Players.GetPlayer;
using EurovisionOnMars.Api.Test.TestData.Players;
using EurovisionOnMars.Dto.Players.GetPlayer;
using EurovisionOnMars.Entity.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Players.GetPlayer;

public class GetPlayerServiceTest
{
    private readonly Mock<ILogger<GetPlayerService>> _loggerMock;
    private readonly Mock<IGetPlayerRepository> _repositoryMock;
    private readonly Mock<IPlayerMapper> _mapperMock;

    private readonly GetPlayerService _service;

    public GetPlayerServiceTest()
    {
        _loggerMock = new Mock<ILogger<GetPlayerService>>();
        _repositoryMock = new Mock<IGetPlayerRepository>();
        _mapperMock = new Mock<IPlayerMapper>();

        _service = new GetPlayerService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetPlayer_PlayerExists_ReturnsDto()
    {
        // arrange
        var username = PlayerTestData.Username;
        var player = PlayerFactory.CreateInitialPlayer();
        var expectedDto = CreatePlayerDto();

        _repositoryMock
            .Setup(r => r.GetPlayer(
                It.Is<Username>(u => u.Value == username)))
            .ReturnsAsync(player);

        _mapperMock
            .Setup(m => m.ToDto(player))
            .Returns(expectedDto);

        // act
        var actualDto = await _service.GetPlayer(username);

        // assert
        Assert.Equal(expectedDto, actualDto);

        _repositoryMock.Verify(
            r => r.GetPlayer(
                It.Is<Username>(u => u.Value == username)),
            Times.Once);

        _mapperMock.Verify(
            m => m.ToDto(player),
            Times.Once);
    }

    [Fact]
    public async Task GetPlayer_PlayerDoesNotExist_ThrowsKeyNotFoundException()
    {
        // arrange
        var username = PlayerTestData.Username;

        _repositoryMock
            .Setup(r => r.GetPlayer(It.IsAny<Username>()))
            .ReturnsAsync((Player?)null);

        // act
        var action = () => _service.GetPlayer(username);

        // assert
        await Assert.ThrowsAsync<KeyNotFoundException>(action);

        _mapperMock.Verify(
            m => m.ToDto(It.IsAny<Player>()),
            Times.Never);
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

        _mapperMock.Verify(
            m => m.ToDto(It.IsAny<Player>()),
            Times.Never);
    }

    private static PlayerDto CreatePlayerDto()
    {
        return new PlayerDto
        {
            Id = PlayerTestData.Id,
            Username = PlayerTestData.Username,
        };
    }
}