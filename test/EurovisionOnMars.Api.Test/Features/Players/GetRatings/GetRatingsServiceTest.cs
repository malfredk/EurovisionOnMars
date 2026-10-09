using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Api.Test.TestData.Players;
using EurovisionOnMars.Dto.Players.GetRatings;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.Players.GetRatings;

public class GetRatingsServiceTest
{
    private const int PlayerId = 1234;

    private readonly Mock<ILogger<GetRatingsService>> _loggerMock;
    private readonly Mock<IGetRatingsRepository> _repositoryMock;
    private readonly Mock<IPlayerRatingMapper> _mapperMock;

    private readonly GetRatingsService _service;

    public GetRatingsServiceTest()
    {
        _loggerMock = new Mock<ILogger<GetRatingsService>>();
        _repositoryMock = new Mock<IGetRatingsRepository>();
        _mapperMock = new Mock<IPlayerRatingMapper>();

        _service = new GetRatingsService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetRatingsByPlayerId_ReturnsRatingsInPredictedRankOrder()
    {
        // arrange
        var player = PlayerFactory.CreatePlayerWith2RatedAndDemotedAnd2InitialRatings();
        var ratings = player.PlayerRatings.ToImmutableList();

        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ratings);

        var dto1 = CreateDto(0);
        var dto2 = CreateDto(1);
        var dto3 = CreateDto(2);
        var dto4 = CreateDto(3);

        _mapperMock
            .Setup(m => m.ToDto(ratings[0]))
            .Returns(dto1);
        _mapperMock
            .Setup(m => m.ToDto(ratings[1]))
            .Returns(dto2);
        _mapperMock
            .Setup(m => m.ToDto(ratings[2]))
            .Returns(dto3);
        _mapperMock
            .Setup(m => m.ToDto(ratings[3]))
            .Returns(dto4);

        var expected = new List<PlayerRatingDto>
        {
            dto2, // predicted rank 1
            dto1, // predicted rank 2
            dto4, // no predicted rank
            dto3, // no predicted rank and high country number
        }.ToImmutableList();

        // act
        var actual = await _service.GetRatingsByPlayerId(PlayerId);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetRatingsByPlayerId_CallsRepository()
    {
        // arrange
        var player = PlayerFactory.CreateInitialPlayer();
        var ratings = player.PlayerRatings.ToImmutableList();

        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ratings);

        _mapperMock
            .Setup(m => m.ToDto(ratings[0]))
            .Returns(CreateDto(0));

        // act
        var actual = await _service.GetRatingsByPlayerId(PlayerId);

        // assert
        _repositoryMock.Verify(
            r => r.GetRatingsByPlayerId(PlayerId),
            Times.Once);
    }

    [Fact]
    public async Task GetRatingsByPlayerId_MapsAllRatings()
    {
        // arrange
        var player = PlayerFactory.CreatePlayerWith2RatedAndDemotedAnd2InitialRatings();

        var ratings = player.PlayerRatings
            .ToImmutableList();

        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ratings);

        var dto = CreateDto(0);
        _mapperMock
            .Setup(m => m.ToDto(It.IsAny<PlayerRating>()))
            .Returns(dto);

        // act
        await _service.GetRatingsByPlayerId(PlayerId);

        // assert
        _mapperMock.Verify(
            m => m.ToDto(It.IsAny<PlayerRating>()),
            Times.Exactly(4));
        foreach (var rating in ratings)
        {
            _mapperMock.Verify(
                m => m.ToDto(rating),
                Times.Once);
        }
    }

    [Fact]
    public async Task GetRatingsByPlayerId_NoRatings_ThrowsKeyNotFoundException()
    {
        // arrange
        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ImmutableList<PlayerRating>.Empty);

        // act
        var action = () => _service.GetRatingsByPlayerId(PlayerId);

        // assert
        await Assert.ThrowsAsync<KeyNotFoundException>(action);

        _mapperMock.Verify(
            m => m.ToDto(It.IsAny<PlayerRating>()),
            Times.Never);
    }

    private static PlayerRatingDto CreateDto(int id)
    {
        return new PlayerRatingDto
        {
            Id = id,
            Prediction = new PredictionDto
            {
                Id = 666,
            },
            Country = new PlayerRatingCountryDto
            {
                Number = CountryTestData.Number,
                Name = CountryTestData.Name
            }
        };
    }
}