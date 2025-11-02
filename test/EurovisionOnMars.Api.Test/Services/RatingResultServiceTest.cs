using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class RatingResultServiceTest
{
    private readonly Mock<IPlayerRatingRepository> _ratingRepositoryMock;
    private readonly Mock<IRatingGameResultRepository> _ratingResultRepositoryMock;
    private readonly Mock<ILogger<RatingGameResultService>> _loggerMock;
    private readonly RatingGameResultService _service;

    public RatingResultServiceTest()
    {
        _ratingRepositoryMock = new Mock<IPlayerRatingRepository>();
        _ratingResultRepositoryMock = new Mock<IRatingGameResultRepository>();
        _loggerMock = new Mock<ILogger<RatingGameResultService>>();

        _service = new RatingGameResultService(
            _ratingRepositoryMock.Object,
            _ratingResultRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async void CalculateRatingResults()
    {
        // arrange
        var playerId = 672;

        var country1stPlace = CreateCountry(1);
        var country2ndPlace = CreateCountry(2);
        var country3rdPlace = CreateCountry(3);
        var country5thPlace = CreateCountry(5);
        var country11thPlace = CreateCountry(11);
        var country14thPlace = CreateCountry(14);
        var country20thPlace = CreateCountry(20);

        var rating1stPlace = CreateRating(country1stPlace, 1);
        var ratingShared2ndPlace1 = CreateRating(country2ndPlace, 2);
        var ratingShared2ndPlace2 = CreateRating(country3rdPlace, 2);
        var rating11thPlace = CreateRating(country11thPlace, 11);
        var rating6thPlace = CreateRating(country14thPlace, 6);
        var rating17thPlace = CreateRating(country5thPlace, 17);
        var ratingNull = CreateRating(country20thPlace, null);

        _ratingRepositoryMock.Setup(m => m.GetPlayerRatingsByPlayerId(playerId))
            .ReturnsAsync(new List<PlayerRating>()
            { 
                rating1stPlace,
                ratingShared2ndPlace1,
                ratingShared2ndPlace2,
                rating11thPlace,
                rating6thPlace,
                rating17thPlace,
                ratingNull
            }.ToImmutableList());

        // act
        await _service.CalculateRatingResults(playerId);

        // assert

        // correct 1st place rating
        Assert.Equal(0, rating1stPlace.RatingGameResult.RankDifference);
        Assert.Equal(-25, rating1stPlace.RatingGameResult.BonusPoints);

        // correct shared 2nd place rating
        Assert.Equal(0, ratingShared2ndPlace1.RatingGameResult.RankDifference);
        Assert.Equal(0, ratingShared2ndPlace1.RatingGameResult.BonusPoints);

        // wrong shared 2nd place rating
        Assert.Equal(1, ratingShared2ndPlace2.RatingGameResult.RankDifference);
        Assert.Equal(0, ratingShared2ndPlace2.RatingGameResult.BonusPoints);

        // correct 11th place rating
        Assert.Equal(0, rating11thPlace.RatingGameResult.RankDifference);
        Assert.Equal(0, rating11thPlace.RatingGameResult.BonusPoints);

        // wrong 6th place rating
        Assert.Equal(8, rating6thPlace.RatingGameResult.RankDifference);
        Assert.Equal(0, rating6thPlace.RatingGameResult.BonusPoints);

        // wrong 17th place rating
        Assert.Equal(-12, rating17thPlace.RatingGameResult.RankDifference);
        Assert.Equal(0, rating17thPlace.RatingGameResult.BonusPoints);

        // no rating
        Assert.Equal(26, ratingNull.RatingGameResult.RankDifference);
        Assert.Equal(0, ratingNull.RatingGameResult.BonusPoints);

        _ratingResultRepositoryMock.Verify(m => m.UpdateRatingGameResult(It.IsAny<RatingGameResult>()), Times.Exactly(7));
    }

    [Fact]
    public async void CalculateRatingResults_MissingCountryRank()
    {
        // arrange
        var playerId = 67;
        var country = CreateCountry(null);
        var rating = CreateRating(country, 26);

        _ratingRepositoryMock.Setup(m => m.GetPlayerRatingsByPlayerId(playerId))
            .ReturnsAsync(new List<PlayerRating>() { rating }.ToImmutableList());

        // act and assert
        await Assert.ThrowsAsync<Exception>(async () => await _service.CalculateRatingResults(playerId));
    }

    private static Country CreateCountry(int? rank)
    {
        return new Country
        {
            Number = 45,
            Name = "tets",
            ActualRank = rank,
        };
    }
    
    private static PlayerRating CreateRating(Country country, int? rank)
    {
        return new PlayerRating
        {
            Prediction = CreatePrediction(rank),
            Country = country,
            RatingGameResult = new RatingGameResult()
        };
    }

    private static Prediction CreatePrediction(int? calculatedRank)
    {
        return new Prediction
        {
            CalculatedRank = calculatedRank
        };
    }
}