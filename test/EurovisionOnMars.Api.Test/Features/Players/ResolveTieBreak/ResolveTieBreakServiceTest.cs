using EurovisionOnMars.Api.Features.Players;
using EurovisionOnMars.Api.Features.Players.ResolveTieBreak;
using EurovisionOnMars.Api.Test.TestData.Players;
using EurovisionOnMars.Dto.Players.ResolveTieBreak;
using EurovisionOnMars.Entity.Players;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features.Players.ResolveTieBreak;

public class ResolveTieBreakServiceTest
{
    private const int PlayerId = 123;

    private readonly Mock<ILogger<ResolveTieBreakService>> _loggerMock;
    private readonly Mock<IRatingTimeValidator> _ratingTimeValidatorMock;
    private readonly Mock<IResolveTieBreakRepository> _repositoryMock;

    private readonly ResolveTieBreakService _service;

    public ResolveTieBreakServiceTest()
    {
        _loggerMock = new Mock<ILogger<ResolveTieBreakService>>();
        _ratingTimeValidatorMock = new Mock<IRatingTimeValidator>();
        _repositoryMock = new Mock<IResolveTieBreakRepository>();

        _service = new ResolveTieBreakService(
            _loggerMock.Object,
            _ratingTimeValidatorMock.Object,
            _repositoryMock.Object);
    }

    [Fact]
    public async Task UpdateTieBreakDemotions_ValidRequest_ResolvesTieBreak()
    {
        // arrange
        var player = PlayerFactory.CreatePlayerWith2TiedRatings();
        var request = CreateValidSwapRequest(player);

        _repositoryMock
            .Setup(r => r.GetPlayer(PlayerId))
            .ReturnsAsync(player);

        // act
        await _service.UpdateTieBreakDemotions(
            PlayerId,
            request);

        // assert
        Assert.Equal(1, player.PlayerRatings[0].Prediction.TieBreakDemotion!.Value);
        Assert.Equal(0, player.PlayerRatings[0].Prediction.TieBreakDemotion!.Value);

        _ratingTimeValidatorMock.Verify(
            v => v.EnsureRatingIsOpen(),
            Times.Once);

        _repositoryMock.Verify(
            r => r.GetPlayer(PlayerId),
            Times.Once);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    [Fact]
    public async Task UpdateTieBreakDemotions_PlayerDoesNotExist_ThrowsKeyNotFoundException()
    {
        // arrange
        var request = CreateDefaultRequest();

        _repositoryMock
            .Setup(r => r.GetPlayer(PlayerId))
            .ReturnsAsync((Player?)null);

        // act
        var action = () =>
            _service.UpdateTieBreakDemotions(PlayerId, request);

        // assert
        await Assert.ThrowsAsync<KeyNotFoundException>(action);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    [Fact]
    public async Task UpdateTieBreakDemotions_RatingIsClosed_DoesNotGetPlayerNorSaveChanges()
    {
        // arrange
        var request = CreateDefaultRequest();

        _ratingTimeValidatorMock
            .Setup(v => v.EnsureRatingIsOpen())
            .Throws<Exception>();

        // act
        var action = () =>
            _service.UpdateTieBreakDemotions(PlayerId, request);

        // assert
        await Assert.ThrowsAsync<Exception>(action);

        _repositoryMock.Verify(
            r => r.GetPlayer(It.IsAny<int>()),
            Times.Never);

        _repositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Never);
    }

    private static ResolveTieBreakRequestDto CreateValidSwapRequest(Player player)
    {
        var ratings = player.PlayerRatings;
        List<int> orderedIds = [
            ratings[1].Prediction.Id,
            ratings[0].Prediction.Id
        ];
        return new ResolveTieBreakRequestDto
        {
            OrderedPredictionIds = orderedIds
        };
    }

    private static ResolveTieBreakRequestDto CreateDefaultRequest()
    {
        return new ResolveTieBreakRequestDto
        {
            OrderedPredictionIds = [1, 2]
        };
    }
}