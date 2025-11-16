using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Api.Test.Features;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class RatingGameResultServiceTest
{
    private readonly Mock<IRatingGameResultRepository> _ratingGameResultRepositoryMock;
    private readonly Mock<IPlayerRatingService> _playerRatingServiceMock;
    private readonly Mock<ILogger<RatingGameResultService>> _loggerMock;
    private readonly RatingGameResultService _service;

    public RatingGameResultServiceTest()
    {
        _ratingGameResultRepositoryMock = new Mock<IRatingGameResultRepository>();
        _playerRatingServiceMock = new Mock<IPlayerRatingService>();
        _loggerMock = new Mock<ILogger<RatingGameResultService>>();

        _service = new RatingGameResultService(
            _ratingGameResultRepositoryMock.Object,
            _playerRatingServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetRatingGameResults()
    {
        // arrange
        var ratingGameResult1 = CreateRatingGameResult(1);
        var ratingGameResult2 = CreateRatingGameResult(2);
        var ratingGameResults = new List<RatingGameResult>
        {
            ratingGameResult1,
            ratingGameResult2
        }.ToImmutableList();

        _ratingGameResultRepositoryMock.Setup(m => m.GetRatingGameResults(Utils.PLAYER_ID))
            .ReturnsAsync(ratingGameResults);

        // act
        var actualResults = await _service.GetRatingGameResults(Utils.PLAYER_ID);

        // assert
        Assert.Equal(ratingGameResults, actualResults);

        _ratingGameResultRepositoryMock
            .Verify(m => m.GetRatingGameResults(Utils.PLAYER_ID), Times.Once);
    }

    [Fact]
    public async Task CalculateRatingGameResults()
    {
        // arrange
        var rating1 = CreatePlayerRating(1, null);
        var rating2 = CreatePlayerRating(5, 6);
        var ratings = new List<PlayerRating>
        {
            rating1,
            rating2
        };
        _playerRatingServiceMock.Setup(m => m.GetAllPlayerRatings())
            .ReturnsAsync(ratings);

        // act
        await _service.CalculateRatingGameResults();

        // assert
        _playerRatingServiceMock
            .Verify(m => m.GetAllPlayerRatings(), Times.Once);
        _ratingGameResultRepositoryMock
            .Verify(m => m.UpdateRatingGameResult(rating1.RatingGameResult), Times.Once);
        _ratingGameResultRepositoryMock
            .Verify(m => m.UpdateRatingGameResult(rating2.RatingGameResult), Times.Once);
    }

    [Theory]
    [InlineData(1, 9)]
    [InlineData(null, 26)]
    [InlineData(10, 0)]
    [InlineData(20, -10)]
    public void CalculateRankDifference_Valid(int? calculatedRank, int expectedRankDifference)
    {
        // arrange
        int actualRank = 10;
        var playerRating = CreatePlayerRating(actualRank, calculatedRank);

        // act
        _service.CalculateRankDifference(playerRating);

        // assert
        Assert.Equal(expectedRankDifference, playerRating.RatingGameResult.RankDifference);
    }

    [Theory]
    [InlineData(1, -25)]
    [InlineData(2, -18)]
    [InlineData(3, -15)]
    [InlineData(4, -12)]
    [InlineData(5, -10)]
    [InlineData(6, -8)]
    [InlineData(7, -6)]
    [InlineData(8, -4)]
    [InlineData(9, -2)]
    [InlineData(10, -1)]
    [InlineData(11, 0)]
    [InlineData(26, 0)]
    public void CalculateBonusPoints_ZeroRankDifferenceAndUniqueRank(int calculatedRank, int expectedBonusPoints)
    {
        // arrange
        var rating = CreatePlayerRatingWithRankDifference(calculatedRank, 0);
        var otherRating = CreatePlayerRating(17, 17);
        var otherRatingWithoutCalculatedRank = CreatePlayerRating();

        var ratings = new List<PlayerRating>
        {
            rating,
            otherRating,
            otherRatingWithoutCalculatedRank,
        };

        // act
        _service.CalculateBonusPoints(rating, ratings);

        // assert
        Assert.Equal(expectedBonusPoints, rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void CalculateBonusPoints_ZeroRankDifferenceAndDuplicateRank()
    {
        // arrange
        var rating = CreatePlayerRatingWithRankDifference(1, 0);
        var otherRating = CreatePlayerRating(10, 1);

        var ratings = new List<PlayerRating>
        {
            rating,
            otherRating
        };

        // act
        _service.CalculateBonusPoints(rating, ratings);

        // assert
        Assert.Equal(0, rating.RatingGameResult.BonusPoints);
    }

    [Fact]
    public void CalculateBonusPoints_NonZeroRankDifference()
    {
        // arrange
        var rating = CreatePlayerRatingWithRankDifference(1, 23);

        var ratings = new List<PlayerRating>
        {
            rating
        };

        // act
        _service.CalculateBonusPoints(rating, ratings);

        // assert
        Assert.Equal(0, rating.RatingGameResult.BonusPoints);
    }

    private RatingGameResult CreateRatingGameResult(int id)
    {
        var playerRating = CreatePlayerRating();
        return new RatingGameResult(playerRating)
        {
            Id = id
        };
    }

    private PlayerRating CreatePlayerRating()
    {
        var player = Utils.CreateInitialPlayerWithOneCountry();
        return player.PlayerRatings.First();
    }

    private PlayerRating CreatePlayerRating(int actualRank, int? calculatedRank)
    {
        var rating = CreatePlayerRating();

        rating.Country.SetActualRank(actualRank);
        rating.Prediction.SetCalculatedRank(calculatedRank);

        return rating;
    }

    private PlayerRating CreatePlayerRatingWithRankDifference(int calculatedRank, int rankDifference)
    {
        var rating = CreatePlayerRating(23, calculatedRank);

        rating.RatingGameResult.RankDifference = rankDifference;

        return rating;
    }
}