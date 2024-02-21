using Castle.Core.Logging;
using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.CustomException;
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
        var player1 = new Player { Username = "alice" };
        var player2 = new Player{ Username = "bob" };
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
        var expectedPlayer = new Player{ Username = "sam" };

        _repositoryMock.Setup(r => r.GetPlayer(id))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(id);

        // assert
        Assert.Equal(actualPlayer, expectedPlayer);

        _repositoryMock.Verify(r => r.GetPlayer(id), Times.Once);
    }

    [Fact]
    public async void GetPlayer_NotValidId()
    {
        // arrange
        var id = 14;

        _repositoryMock.Setup(r => r.GetPlayer(id))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetPlayer(id));

        _repositoryMock.Verify(r => r.GetPlayer(id), Times.Once);
    }

    [Fact]
    public async void GetPlayer_ValidUsername()
    {
        // arrange
        var username = "nisse";
        var expectedPlayer = new Player { Username = username };

        _repositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(username);

        // assert
        Assert.Equal(actualPlayer, expectedPlayer);

        _repositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
    }

    [Fact]
    public async void GetPlayer_NotValidUsername()
    {
        // arrange
        var username = "";

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetPlayer(username));

        _repositoryMock.Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async void GetPlayer_NotExistingUsername()
    {
        // arrange
        var username = "nope";

        _repositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetPlayer(username));
        
        _repositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
    }

    [Fact]
    public async void CreatePlayer_ValidUsername()
    {
        // arrange
        var username = "hiæøÅ1278";
        var expectedPlayer = new Player{ Username = username };

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

    [Theory]
    [InlineData("")]
    [InlineData("hei ho")]
    [InlineData("j*n")]
    [InlineData("=ndwnfks")]
    [InlineData("tretten123456")]
    public async void CreatePlayer_NotValidUsername(string username)
    {
        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.CreatePlayer(username));
        
        _repositoryMock.Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
        _repositoryMock.Verify(r => r.CreatePlayer(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async void CreatePlayer_ExistingUsername()
    {
        // arrange
        var username = "nope";
        var existingPlayer = new Player{ Username = username };

        _repositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync(existingPlayer);

        // act and assert
        await Assert.ThrowsAsync<DuplicateUsernameException>(async () => await _service.CreatePlayer(username));

        _repositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
        _repositoryMock.Verify(r => r.CreatePlayer(It.IsAny<string>()), Times.Never());
    }
}
