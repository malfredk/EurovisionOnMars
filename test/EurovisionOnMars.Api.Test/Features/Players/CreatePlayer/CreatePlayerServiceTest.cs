using EurovisionOnMars.Api.Features.Players.CreatePlayer;
using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Api.Test.TestData.Players;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Players.CreatePlayer;

public class CreatePlayerServiceTest
{
    private readonly Mock<ILogger<CreatePlayerService>> _loggerMock;
    private readonly Mock<ICreatePlayerRepository> _repositoryMock;
    private readonly CreatePlayerService _service;

    public CreatePlayerServiceTest()
    {
        _loggerMock = new Mock<ILogger<CreatePlayerService>>();
        _repositoryMock = new Mock<ICreatePlayerRepository>();

        _service = new CreatePlayerService(
            _loggerMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task CreatePlayer_ValidUsername_CreatesPlayer()
    {
        // arrange
        var username = PlayerTestData.Username;
        var countries = CountryFactory.CreateInitialSingletonList();

        _repositoryMock
            .Setup(r => r.UsernameExists(It.IsAny<Username>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.GetCountries())
            .ReturnsAsync(countries);

        // act
        await _service.CreatePlayer(username);

        // assert
        _repositoryMock.Verify(
            r => r.UsernameExists(
                It.Is<Username>(u => u.Value == username)),
            Times.Once);

        _repositoryMock.Verify(
            r => r.GetCountries(),
            Times.Once);

        _repositoryMock.Verify(
            r => r.AddPlayer(
                It.Is<Player>(p =>
                    p.Username.Value == username &&
                    p.PlayerRatings.Count == countries.Count)),
            Times.Once);
    }

    [Fact]
    public async Task CreatePlayer_UsernameAlreadyExists_ThrowsDuplicateUsernameException()
    {
        // arrange
        var username = PlayerTestData.Username;

        _repositoryMock
            .Setup(r => r.UsernameExists(It.IsAny<Username>()))
            .ReturnsAsync(true);

        // act
        var action = () => _service.CreatePlayer(username);

        // assert
        await Assert.ThrowsAsync<DuplicateUsernameException>(action);

        _repositoryMock.Verify(
            r => r.GetCountries(),
            Times.Never);

        _repositoryMock.Verify(
            r => r.AddPlayer(It.IsAny<Player>()),
            Times.Never);
    }

    [Fact]
    public async Task CreatePlayer_InvalidUsername_DoesNotAccessRepository()
    {
        // arrange
        var invalidUsername = PlayerTestData.Username;

        // act
        var action = () => _service.CreatePlayer(invalidUsername);

        // assert
        await Assert.ThrowsAsync<ArgumentException>(action);

        _repositoryMock.Verify(
            r => r.UsernameExists(It.IsAny<Username>()),
            Times.Never);

        _repositoryMock.Verify(
            r => r.GetCountries(),
            Times.Never);

        _repositoryMock.Verify(
            r => r.AddPlayer(It.IsAny<Player>()),
            Times.Never);
    }
}