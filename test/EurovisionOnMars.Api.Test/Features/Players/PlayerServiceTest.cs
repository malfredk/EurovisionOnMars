using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Players;

public class PlayerServiceTest
{
    private readonly Mock<IPlayerRepository> _playerRepositoryMock;
    private readonly Mock<ICountryService> _countryServiceMock;
    private readonly Mock<ILogger<PlayerService>> _loggerMock;
    private readonly PlayerService _service;

    public PlayerServiceTest()
    {
        _playerRepositoryMock = new Mock<IPlayerRepository>();
        _countryServiceMock = new Mock<ICountryService>();
        _loggerMock = new Mock<ILogger<PlayerService>>();

        _service = new PlayerService(
            _playerRepositoryMock.Object, 
            _countryServiceMock.Object,
            _loggerMock.Object
            );
    }

    // tests for getting player by id

    [Fact]
    public async Task GetPlayer_ValidId()
    {
        // arrange
        var expectedPlayer = Utils.CreateInitialPlayerWithOneCountry();

        _playerRepositoryMock.Setup(r => r.GetPlayer(Utils.PLAYER_ID))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(Utils.PLAYER_ID);

        // assert
        Assert.Equal(expectedPlayer, actualPlayer);

        _playerRepositoryMock.Verify(r => r.GetPlayer(Utils.PLAYER_ID), Times.Once);
    }

    [Fact]
    public async Task GetPlayer_InvalidId()
    {
        // arrange
        _playerRepositoryMock.Setup(r => r.GetPlayer(Utils.PLAYER_ID))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetPlayer(Utils.PLAYER_ID)
        );

        _playerRepositoryMock.Verify(r => r.GetPlayer(Utils.PLAYER_ID), Times.Once);
    }

    // tests for getting player by username

    [Fact]
    public async Task GetPlayer_ValidUsername()
    {
        // arrange
        var expectedPlayer = Utils.CreateInitialPlayerWithOneCountry();

        _playerRepositoryMock.Setup(r => r.GetPlayer(Utils.PLAYER_USERNAME))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(Utils.PLAYER_USERNAME);

        // assert
        Assert.Equal(expectedPlayer, actualPlayer);

        _playerRepositoryMock.Verify(r => r.GetPlayer(Utils.PLAYER_USERNAME), Times.Once);
    }

    [Fact]
    public async Task GetPlayer_InvalidUsername()
    {
        // arrange
        var username = "";

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.GetPlayer(username)
        );

        _playerRepositoryMock
            .Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async Task GetPlayer_NotExistingUsername()
    {
        // arrange
        _playerRepositoryMock.Setup(r => r.GetPlayer(Utils.PLAYER_USERNAME))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetPlayer(Utils.PLAYER_USERNAME)
        );
        
        _playerRepositoryMock
            .Verify(r => r.GetPlayer(Utils.PLAYER_USERNAME), Times.Once);
    }

    // tests for creating player

    [Fact]
    public async Task CreatePlayer_ValidUsername()
    {
        // arrange
        var country = Utils.CreateInitialCountry();
        var expectedPlayer = Utils.CreateInitialPlayerWithOneCountry();
        Player capturedNewPlayer = null!;

        _playerRepositoryMock.Setup(r => r.GetPlayer(Utils.PLAYER_USERNAME))
            .ReturnsAsync((Player)null);
        _countryServiceMock.Setup(r => r.GetCountries())
            .ReturnsAsync([country]);
        _playerRepositoryMock.Setup(r => r.CreatePlayer(It.IsAny<Player>()))
            .Callback<Player>(input => capturedNewPlayer = input)
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.CreatePlayer(Utils.PLAYER_USERNAME);

        // assert
        Assert.Equal(expectedPlayer, actualPlayer);
        Assert.Equal(Utils.PLAYER_USERNAME, capturedNewPlayer.Username);
        Assert.Equal(country, capturedNewPlayer.PlayerRatings.First().Country);
        Assert.Single(capturedNewPlayer.PlayerRatings);

        _playerRepositoryMock.Verify(r => r.GetPlayer(Utils.PLAYER_USERNAME), Times.Once);
        _countryServiceMock.Verify(r => r.GetCountries(), Times.Once);
        _playerRepositoryMock.Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Once);
    }

    [Fact]
    public async Task CreatePlayer_NotValidUsername()
    {
        // arrange
        var username = "= injection attack";

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.CreatePlayer(username)
        );
        
        _playerRepositoryMock
            .Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
        _playerRepositoryMock
            .Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Never());
    }

    [Fact]
    public async Task CreatePlayer_ExistingUsername()
    {
        // arrange
        var existingPlayer = Utils.CreateInitialPlayerWithOneCountry();

        _playerRepositoryMock.Setup(r => r.GetPlayer(Utils.PLAYER_USERNAME))
            .ReturnsAsync(existingPlayer);

        // act and assert
        await Assert.ThrowsAsync<DuplicateUsernameException>(
            async () => await _service.CreatePlayer(Utils.PLAYER_USERNAME)
        );

        _playerRepositoryMock
            .Verify(r => r.GetPlayer(Utils.PLAYER_USERNAME), Times.Once);
        _countryServiceMock
            .Verify(r => r.GetCountries(), Times.Never());
        _playerRepositoryMock
            .Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Never());
    }
}