using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Dto.Players.GetRatings;
using EurovisionOnMars.TestSupport.Countries;
using EurovisionOnMars.TestSupport.Players;
using Microsoft.Extensions.Logging;
using Moq;

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
        var player = PlayerFactory.CreateInitial(3);
        var ratings = player.PlayerRatings;

        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ratings);

        var firstRankDto = CreateDto(predictedRank: 1);
        var secondRankDto = CreateDto(predictedRank: 2);
        var thirdRankDto = CreateDto(predictedRank: 3);

        _mapperMock
            .Setup(m => m.ToDto(ratings[0]))
            .Returns(thirdRankDto);
        _mapperMock
            .Setup(m => m.ToDto(ratings[1]))
            .Returns(firstRankDto);
        _mapperMock
            .Setup(m => m.ToDto(ratings[2]))
            .Returns(secondRankDto);

        var expected = new List<PlayerRatingDto>
        {
            firstRankDto,
            secondRankDto,
            thirdRankDto,
        };

        // act
        var actual = await _service.GetRatingsByPlayerId(PlayerId);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetRatingsByPlayerId_SortsNoRankLast()
    {
        // arrange
        var player = PlayerFactory.CreateInitial(3);
        var ratings = player.PlayerRatings;

        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ratings);

        var firstRankDto = CreateDto(predictedRank: 1);
        var secondRankDto = CreateDto(predictedRank: 2);
        var noRankDto = CreateDto(predictedRank: null);

        _mapperMock
            .Setup(m => m.ToDto(ratings[0]))
            .Returns(noRankDto);
        _mapperMock
            .Setup(m => m.ToDto(ratings[1]))
            .Returns(firstRankDto);
        _mapperMock
            .Setup(m => m.ToDto(ratings[2]))
            .Returns(secondRankDto);

        var expected = new List<PlayerRatingDto>
        {
            firstRankDto,
            secondRankDto,
            noRankDto,
        };

        // act
        var actual = await _service.GetRatingsByPlayerId(PlayerId);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetRatingsByPlayerId_SortsFirstByRankThenByNumber()
    {
        // arrange
        var player = PlayerFactory.CreateInitial(3);
        var ratings = player.PlayerRatings;

        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ratings);

        var rankedNumber1Dto = CreateDto(predictedRank: 1, number: 1);
        var noRankNumber2Dto = CreateDto(predictedRank: null, number: 2);
        var noRankNumber3Dto = CreateDto(predictedRank: null, number: 3);

        _mapperMock
            .Setup(m => m.ToDto(ratings[0]))
            .Returns(noRankNumber3Dto);
        _mapperMock
            .Setup(m => m.ToDto(ratings[1]))
            .Returns(rankedNumber1Dto);
        _mapperMock
            .Setup(m => m.ToDto(ratings[2]))
            .Returns(noRankNumber2Dto);

        var expected = new List<PlayerRatingDto>
        {
            rankedNumber1Dto,
            noRankNumber2Dto,
            noRankNumber3Dto,
        };

        // act
        var actual = await _service.GetRatingsByPlayerId(PlayerId);

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetRatingsByPlayerId_GetsRatings()
    {
        // arrange
        var player = PlayerFactory.CreateInitialWithOneRating();
        var ratings = player.PlayerRatings;

        _repositoryMock
            .Setup(r => r.GetRatingsByPlayerId(PlayerId))
            .ReturnsAsync(ratings);

        _mapperMock
            .Setup(m => m.ToDto(It.IsAny<PlayerRating>()))
            .Returns(CreateDto(1));

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
        var player = PlayerFactory.CreateInitial(3);

        var ratings = player.PlayerRatings;

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
            .ReturnsAsync(new List<PlayerRating> { });

        // act
        var action = () => _service.GetRatingsByPlayerId(PlayerId);

        // assert
        await Assert.ThrowsAsync<KeyNotFoundException>(action);

        _mapperMock.Verify(
            m => m.ToDto(It.IsAny<PlayerRating>()),
            Times.Never);
    }

    private static PlayerRatingDto CreateDto(int? predictedRank, int number = CountryTestData.Number)
    {
        return new PlayerRatingDto
        {
            Id = 66,
            Prediction = new PredictionDto
            {
                Id = 666,
                PredictedRank = predictedRank
            },
            Country = new PlayerRatingCountryDto
            {
                Number = number,
                Name = CountryTestData.Name
            }
        };
    }
}