using EurovisionOnMars.Api.Features.Players.GetRatingGameResults;
using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Api.Test.TestData.Game;
using EurovisionOnMars.Dto.Players.GetRatingGameResults;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.Players.GetRatingGameResults;

public class GetRatingGameResultsServiceTest
{
    private readonly Mock<ILogger<GetRatingGameResultsService>> _loggerMock;
    private readonly Mock<IGetRatingGameResultsRepository> _repositoryMock;
    private readonly Mock<IRatingGameResultMapper> _mapperMock;

    private readonly GetRatingGameResultsService _service;

    public GetRatingGameResultsServiceTest()
    {
        _loggerMock = new Mock<ILogger<GetRatingGameResultsService>>();
        _repositoryMock = new Mock<IGetRatingGameResultsRepository>();
        _mapperMock = new Mock<IRatingGameResultMapper>();

        _service = new GetRatingGameResultsService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetRatingGameResults_ReturnsMappedDtos()
    {
        // arrange
        const int playerId = 1234;

        var gameScenario =
            GameScenarioFactory.CreateInitialGame();
        var ratings = gameScenario.Players[0].PlayerRatings.ToImmutableList();

        _repositoryMock
            .Setup(r => r.GetRatings(playerId))
            .ReturnsAsync(ratings);

        var dto1 = CreateDto(1);
        var dto2 = CreateDto(2);

        _mapperMock
            .Setup(m => m.ToDto(ratings[0]))
            .Returns(dto1);
        _mapperMock
            .Setup(m => m.ToDto(ratings[1]))
            .Returns(dto2);

        var expected = new[]
        {
            dto1,
            dto2
        }.ToImmutableList();

        // act
        var actual = await _service.GetRatingGameResults(playerId);

        // assert
        Assert.Equal(expected, actual);

        _repositoryMock.Verify(
            r => r.GetRatings(playerId),
            Times.Once);

        _mapperMock.Verify(
            m => m.ToDto(ratings[0]),
            Times.Once);
        _mapperMock.Verify(
            m => m.ToDto(ratings[1]),
            Times.Once);
    }

    private static RatingGameResultDto CreateDto(int rankDifference)
    {
        return new RatingGameResultDto
        {
            RankDifference = rankDifference,
            BonusPoints = 0,
            Country = new RatingGameResultCountryDto
            {
                Name = CountryTestData.Name,
                ActualRank = CountryTestData.Rank
            }
        };
    }
}