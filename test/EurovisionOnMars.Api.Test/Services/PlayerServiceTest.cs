using Castle.Core.Logging;
using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class PlayerServiceTest
{
    private readonly Mock<IPlayerRepository> _repositoryMock;
    private readonly Mock<ILogger<PlayerService>> _loggerMock;
    private readonly PlayerService _service;

    public PlayerServiceTest()
    {
        _repositoryMock = new Mock<IPlayerRepository>();
        _loggerMock = new Mock<ILogger<PlayerService>>();

        _service = new PlayerService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async void GetPlayers()
    {
        // arrange
        var player1 = new Player("alice");
        var player2 = new Player("bob");
        var expectedPlayers = new List<Player>() { 
            player1,
            player2
        }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetPlayers())
            .ReturnsAsync(expectedPlayers);

        // act
        var actualPlayers = await _service.GetPlayers();

        // assert
        Assert.Equal(actualPlayers, expectedPlayers);

        _repositoryMock.Verify(r => r.GetPlayers(), Times.Once);
    }

    [Fact]
    public async void GetPlayer_ValidId()
    {
        // arrange
        var id = 14;
        var expectedPlayer = new Player("sam");

        _repositoryMock.Setup(r => r.GetPlayer(id))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(id);

        // assert
        Assert.Equal(actualPlayer, expectedPlayer);

        _repositoryMock.Verify(r => r.GetPlayer(id), Times.Once);
    }

    [Fact]
    public void GetPlayer_NotValidId()
    {
        // arrange
        var id = 14;

        _repositoryMock.Setup(r => r.GetPlayer(id))
            .ReturnsAsync((Player)null);

        // act and assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetPlayer(id));

        _repositoryMock.Verify(r => r.GetPlayer(id), Times.Once);
    }

    [Fact]
    public async void GetPlayer_ValidUsername()
    {
        // arrange
        var username = "nisse";
        var expectedPlayer = new Player(username);

        _repositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(username);

        // assert
        Assert.Equal(actualPlayer, expectedPlayer);

        _repositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
    }

    [Fact]
    public void GetPlayer_NotValidUsername()
    {
        // arrange
        var username = "";

        // act and assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetPlayer(username));

        _repositoryMock.Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public void GetPlayer_NotExistingUsername()
    {
        // arrange
        var username = "nope";

        _repositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync((Player)null);

        // act and assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetPlayer(username));
        
        _repositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
    }

    [Fact]
    public async void CreatePlayer_ValidUsername()
    {
        // arrange
        var username = "hihi";
        var expectedPlayer = new Player(username);

        _repositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync((Player)null);
        _repositoryMock.Setup(r => r.CreatePlayer(username))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.CreatePlayer(username);

        // assert
        Assert.Equal(actualPlayer, expectedPlayer);

        _repositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
        _repositoryMock.Verify(r => r.CreatePlayer(username), Times.Once);
    }

    [Fact]
    public void CreatePlayer_NotValidUsername()
    {
        // arrange
        var username = "";

        // act and assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreatePlayer(username));
        
        _repositoryMock.Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
        _repositoryMock.Verify(r => r.CreatePlayer(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public void CreatePlayer_ExistingUsername()
    {
        // arrange
        var username = "nope";
        var existingPlayer = new Player(username);

        _repositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync(existingPlayer);

        // act and assert
        Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreatePlayer(username));

        _repositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
        _repositoryMock.Verify(r => r.CreatePlayer(It.IsAny<string>()), Times.Never());
    }
}
