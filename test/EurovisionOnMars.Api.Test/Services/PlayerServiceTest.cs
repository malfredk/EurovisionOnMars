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
    private readonly Mock<IPlayerRepository> _playerRepositoryMock;
    private readonly Mock<ICountryRepository> _countryRepositoryMock;
    private readonly Mock<ILogger<PlayerService>> _loggerMock;
    private readonly PlayerService _service;

    public PlayerServiceTest()
    {
        _playerRepositoryMock = new Mock<IPlayerRepository>();
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _loggerMock = new Mock<ILogger<PlayerService>>();

        _service = new PlayerService(
            _playerRepositoryMock.Object, 
            _countryRepositoryMock.Object,
            _loggerMock.Object
            );
    }

    [Fact]
    public async void GetPlayers()
    {
        // arrange
        var player1 = CreatePlayer("alice", 5);
        var player2 = CreatePlayer("bob", 1);
        var player3 = CreatePlayer("chris", null);
        var player4 = CreatePlayer("krass", 2);

        var players = new List<Player>() { 
            player1,
            player2,
            player3,
            player4
        }.ToImmutableList();
        var expectedPlayers = new List<Player>() {
            player2,
            player4,
            player1,
            player3
        }.ToImmutableList();

        _playerRepositoryMock.Setup(r => r.GetPlayers())
            .ReturnsAsync(players);

        // act
        var actualPlayers = await _service.GetPlayers();

        // assert
        Assert.Equal(expectedPlayers, actualPlayers);

        _playerRepositoryMock.Verify(r => r.GetPlayers(), Times.Once);
    }

    [Fact]
    public async void GetPlayer_ValidId()
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
    public async void GetPlayer_InvalidId()
    {
        // arrange
        var id = 14;

        _playerRepositoryMock.Setup(r => r.GetPlayer(id))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetPlayer(id));

        _playerRepositoryMock.Verify(r => r.GetPlayer(id), Times.Once);
    }

    [Fact]
    public async void GetPlayer_ValidUsername()
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
    public async void GetPlayer_InvalidUsername()
    {
        // arrange
        var username = "";

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.GetPlayer(username));

        _playerRepositoryMock.Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
    }

    [Fact]
    public async void GetPlayer_NotExistingUsername()
    {
        // arrange
        var username = "nope";

        _playerRepositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync((Player)null);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetPlayer(username));
        
        _playerRepositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
    }

    [Fact]
    public async void CreatePlayer_ValidUsername()
    {
        // arrange
        var username = "hiæøÅ1278";

        var country1 = CreateCountry(67, "noreg");
        var country2 = CreateCountry(45, "svg");

        var rating1 = CreateRating(67, country1);
        var rating2 = CreateRating(45, country2);

        var expectedPlayer = new Player { Username = "dasikj" };
        var capturedNewPlayer = (Player)null;

        _playerRepositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync((Player)null);
        _countryRepositoryMock.Setup(r => r.GetCountries())
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
        _countryRepositoryMock.Verify(r => r.GetCountries(), Times.Once);
        _playerRepositoryMock.Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Once);
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
        
        _playerRepositoryMock.Verify(r => r.GetPlayer(It.IsAny<string>()), Times.Never());
        _playerRepositoryMock.Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Never());
    }

    [Fact]
    public async void CreatePlayer_ExistingUsername()
    {
        // arrange
        var username = "nope";
        var existingPlayer = new Player{ Username = username };

        _playerRepositoryMock.Setup(r => r.GetPlayer(username))
            .ReturnsAsync(existingPlayer);

        // act and assert
        await Assert.ThrowsAsync<DuplicateUsernameException>(async () => await _service.CreatePlayer(username));

        _playerRepositoryMock.Verify(r => r.GetPlayer(username), Times.Once);
        _countryRepositoryMock.Verify(r => r.GetCountries(), Times.Never());
        _playerRepositoryMock.Verify(r => r.CreatePlayer(It.IsAny<Player>()), Times.Never());
    }

    private static Player CreatePlayer(string username, int? rank)
    {
        return new Player
        {
            Username = username,
            PlayerGameResult = new PlayerGameResult 
            { 
                PlayerId = 1000,
                Rank = rank
            }
        };
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