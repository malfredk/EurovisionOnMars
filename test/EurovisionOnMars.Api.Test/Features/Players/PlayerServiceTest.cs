using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

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
        var id = 14;
        var expectedPlayer = new Player{ Username = "sam" };

        _playerRepositoryMock.Setup(r => r.GetPlayer(id))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(id);

        // assert
        Assert.Equal(expectedPlayer, actualPlayer);

        _playerRepositoryMock.Verify(r => r.GetPlayer(id), Times.Once);
    }

    [Fact]
    public async Task GetPlayer_InvalidId()
    {
        // arrange
        var id = 14;

        _playerRepositoryMock.Setup(r => r.GetPlayer(id))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetPlayer(id)
        );

        _playerRepositoryMock.Verify(r => r.GetPlayer(id), Times.Once);
    }

    // tests for getting player by username

    [Fact]
    public async Task GetPlayer_ValidUsername()
    {
        // arrange
        var username = "nisse";
        var expectedPlayer = new Player { Username = username };

        _playerRepositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.GetPlayer(username);

        // assert
        Assert.Equal(expectedPlayer, actualPlayer);

        _playerRepositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
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
        var username = "nope";

        _playerRepositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetPlayer(username)
        );
        
        _playerRepositoryMock
            .Verify(r => r.GetPlayer(username), Times.Once);
    }

    // tests for creating player

    [Fact]
    public async Task CreatePlayer_ValidUsername()
    {
        // arrange
        var username = "hiæøÅ1278";

        var country1 = CreateCountry(67, "noreg");
        var country2 = CreateCountry(45, "svg");

        var rating1 = CreateRating(67, country1);
        var rating2 = CreateRating(45, country2);

        var expectedPlayer = new Player { Username = "dasikj" };
        Player capturedNewPlayer = null;

        _playerRepositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync((Player)null);
        _countryServiceMock.Setup(r => r.GetCountries())
            .ReturnsAsync(new List<Country> { country1, country2 }.ToImmutableList());
        _playerRepositoryMock.Setup(r => r.CreatePlayer(It.IsAny<Player>()))
            .Callback<Player>(input => capturedNewPlayer = input)
            .ReturnsAsync(expectedPlayer);

        // act
        var actualPlayer = await _service.CreatePlayer(username);

        // assert
        Assert.Equal(expectedPlayer, actualPlayer);
        Assert.Equal(username, capturedNewPlayer.Username);
        Assert.Equal(rating1, capturedNewPlayer.PlayerRatings[0]);
        Assert.Equal(rating2, capturedNewPlayer.PlayerRatings[1]);
        Assert.Equal(2, capturedNewPlayer.PlayerRatings.Count);

        _playerRepositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
        _countryServiceMock.Verify(r => r.GetCountries(), Times.Once);
        _playerRepositoryMock.Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("hei ho")]
    [InlineData("j*n")]
    [InlineData("=ndwnfks")]
    [InlineData("tretten123456")]
    public async Task CreatePlayer_NotValidUsername(string username)
    {
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
        var username = "nope";
        var existingPlayer = new Player{ Username = username };

        _playerRepositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync(existingPlayer);

        // act and assert
        await Assert.ThrowsAsync<DuplicateUsernameException>(
            async () => await _service.CreatePlayer(username)
        );

        _playerRepositoryMock
            .Verify(r => r.GetPlayer(username), Times.Once);
        _countryServiceMock
            .Verify(r => r.GetCountries(), Times.Never());
        _playerRepositoryMock
            .Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Never());
    }

    private static Country CreateCountry(int id, string name)
    {
        return new Country
        {
            Name = name,
            Id = id,
            Number = 78
        };
    }

    private static PlayerRating CreateRating(int countryId, Country country)
    {
        return new PlayerRating
        {
            CountryId = countryId,
            Country = country,
            RatingGameResult = new RatingGameResult()
        };
    }
}