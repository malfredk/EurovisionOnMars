using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class RatingResultServiceTest
{
    private readonly Mock<IRatingRepository> _ratingRepositoryMock;
    private readonly Mock<IRatingResultRepository> _ratingResultRepositoryMock;
    private readonly Mock<ILogger<RatingResultService>> _loggerMock;
    private readonly RatingResultService _service;

    public RatingResultServiceTest()
    {
        _ratingRepositoryMock = new Mock<IRatingRepository>();
        _ratingResultRepositoryMock = new Mock<IRatingResultRepository>();
        _loggerMock = new Mock<ILogger<RatingResultService>>();

        _service = new RatingResultService(
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

        _ratingRepositoryMock.Setup(m => m.GetRatingsByPlayer(playerId))
            .ReturnsAsync(new List<Rating>()
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
        Assert.Equal(0, rating1stPlace.RatingResult.RankingDifference);
        Assert.Equal(-25, rating1stPlace.RatingResult.BonusPoints);

        // correct shared 2nd place rating
        Assert.Equal(0, ratingShared2ndPlace1.RatingResult.RankingDifference);
        Assert.Equal(0, ratingShared2ndPlace1.RatingResult.BonusPoints);

        // wrong shared 2nd place rating
        Assert.Equal(1, ratingShared2ndPlace2.RatingResult.RankingDifference);
        Assert.Equal(0, ratingShared2ndPlace2.RatingResult.BonusPoints);

        // correct 11th place rating
        Assert.Equal(0, rating11thPlace.RatingResult.RankingDifference);
        Assert.Equal(0, rating11thPlace.RatingResult.BonusPoints);

        // wrong 6th place rating
        Assert.Equal(8, rating6thPlace.RatingResult.RankingDifference);
        Assert.Equal(0, rating6thPlace.RatingResult.BonusPoints);

        // wrong 17th place rating
        Assert.Equal(-12, rating17thPlace.RatingResult.RankingDifference);
        Assert.Equal(0, rating17thPlace.RatingResult.BonusPoints);

        // no rating
        Assert.Equal(26, ratingNull.RatingResult.RankingDifference);
        Assert.Equal(0, ratingNull.RatingResult.BonusPoints);

        _ratingResultRepositoryMock.Verify(m => m.UpdateRatingResult(It.IsAny<RatingResult>()), Times.Exactly(7));
    }

    private static Country CreateCountry(int? ranking)
    {
        return new Country
        {
            Number = 45,
            Name = "tets",
            Ranking = ranking,
        };
    }
    
    private static Rating CreateRating(Country country, int? ranking)
    {
        return new Rating
        {
            Ranking = ranking,
            Country = country
        };
    }
}