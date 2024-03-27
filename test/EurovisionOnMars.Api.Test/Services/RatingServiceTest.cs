using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Dto.Requests;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class RatingServiceTest
{
    private const int RATING_ID = 821;
    private const int PLAYER_ID = 657;

    private readonly Mock<IRatingRepository> _repositoryMock;
    private readonly Mock<IRateClosingService> _rateClosingServiceMock;
    private readonly Mock<ILogger<RatingService>> _loggerMock;
    private readonly RatingService _service;

    public RatingServiceTest()
    {
        _repositoryMock = new Mock<IRatingRepository>();
        _rateClosingServiceMock = new Mock<IRateClosingService>();
        _loggerMock = new Mock<ILogger<RatingService>>();

        _service = new RatingService(
            _repositoryMock.Object, 
            _rateClosingServiceMock.Object,
            _loggerMock.Object);
    }

    // tests for getting ratings by player

    [Fact]
    public async void GetRatingsByPlayer()
    {
        // arrange
        var rating1 = CreateRating(1, null, null, 100);
        var rating2 = CreateRating(2, null, 5, 5);
        var rating3 = CreateRating(3, null, 7, 10);
        var rating4 = CreateRating(4, null, 1, 19);
        var rating5 = CreateRating(5, null, 5, 6);
        var rating6 = CreateRating(6, null, null, 89);

        var expectedRatings = new List<Rating>() 
        { 
            rating1,
            rating2,
            rating3,
            rating4,
            rating5,
            rating6
        }.ToImmutableList();
        var sortedExpectedRatings = new List<Rating>()
        {
            rating4,
            rating2, 
            rating5,
            rating3,
            rating6,
            rating1
        }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(expectedRatings);

        // act
        var ratings = await _service.GetRatingsByPlayer(PLAYER_ID);

        // assert
        Assert.Equal(ratings, sortedExpectedRatings);

        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());

        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());
    }

    [Fact]
    public async void GetRatingsByPlayer_NoResults()
    {
        // arrange
        var expectedRatings = new List<Rating>() { }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(expectedRatings);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetRatingsByPlayer(PLAYER_ID));
        
        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());

        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());
    }

    // tests for getting a rating by id

    [Fact]
    public async void GetRating()
    {
        // arrange
        var expectedRating = CreateRating(12, 1, null, 3);

        _repositoryMock.Setup(r => r.GetRating(RATING_ID))
            .ReturnsAsync(expectedRating);

        // act
        var rating = await _service.GetRating(RATING_ID);

        // assert
        Assert.Equal(rating, expectedRating);

        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
    }

    [Fact]
    public async void GetRating_InvalidId()
    {
        // arrange
        var expectedRating = (Rating)null;

        _repositoryMock.Setup(r => r.GetRating(RATING_ID))
            .ReturnsAsync(expectedRating);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetRating(RATING_ID));

        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
    }

    // tests for updating rating

    [Theory]
    [MemberData(nameof(GetTestData))]
    public async void UpdateRating_ValidOtherRating(
        RatingPointsRequestDto requestRatingDto, 
        Rating otherRating,
        Rating expectedUpdatedRating
        )
    {
        // arrange
        var oldRating = CreateRating(RATING_ID, null, null);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(new List<Rating>() { otherRating, oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, requestRatingDto);

        // assert
        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Exactly(2));
        _repositoryMock.Verify(r => r.UpdateRating(otherRating), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    public static IEnumerable<object[]> GetTestData()
    {
        // giving 10 points in category 1, 10 points has already been given in category 3 for another country
        yield return new object[] { 
            CreateRatingRequest(10, 1, 2), 
            CreateRating(1, 6, 2, 10),
            CreateRating(RATING_ID, 10, 1, 2, 13, 1)
        };
        // giving 10 points in category 1, 12 points has already been given in category 1 for another country
        yield return new object[] { 
            CreateRatingRequest(10, 1, 2), 
            CreateRating(1, 12, 2, 5),
            CreateRating(RATING_ID, 10, 1, 2, 13, 1)
        };
        // giving 7 points in category 1, 7 points has already been given in category 1 for another country
        yield return new object[] { 
            CreateRatingRequest(7, 1, 2), 
            CreateRating(1, 7, 3, 6),
            CreateRating(RATING_ID, 7, 1, 2, 10, 1)
        };
    }

    // giving 10 points in category 2, 10 points has already been given in category 2 for the same country
    [Fact]
    public async void UpdateRating_ValidUpdatedRating()
    {
        // arrange
        var ratingRequest = CreateRatingRequest(1, 10, 2);
        var oldRating = CreateRating(RATING_ID, 1, 10, 5, 1000, 67);
        var expectedUpdatedRating = CreateRating(RATING_ID, 1, 10, 2, 13, 1);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(new List<Rating>() { oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    [Fact]
    public async void UpdateRating_CorrectRanking()
    {
        // arrange
        var oldRating = CreateRating(RATING_ID, null, null);
        var ratingRequest = CreateRatingRequest(10, 10, 10);
        var expectedUpdatedRating = CreateRating(RATING_ID, 10, 10, 10, 30, 4);

        var rating1 = CreateRating(1, 1000, null);
        var expectedRating1 = CreateRating(1, 1000, 1);

        var rating2 = CreateRating(2, 1000, null);
        var expectedRating2 = CreateRating(2, 1000, 1);

        var rating3 = CreateRating(3, null, null);
        var expectedRating3 = CreateRating(3, null, null);

        var rating4 = CreateRating(4, 700, null);
        var expectedRating4 = CreateRating(4, 700, 3);

        var rating5 = CreateRating(5, 30, 2);
        var expectedRating5 = CreateRating(5, 30, 4);

        var rating6 = CreateRating(6, 2, 10);
        var expectedRating6 = CreateRating(6, 2, 6);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(new List<Rating>() 
            { 
                oldRating,
                rating1,
                rating2,
                rating3,
                rating4,
                rating5,
                rating6
            }
            .ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once);
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Exactly(7));
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating1), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating2), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating3), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating4), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating5), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating6), Times.Once());
    }

    [Theory]
    [MemberData(nameof(GetTestData_Invalid))]
    public async void UpdateRating_Invalid(RatingPointsRequestDto ratingRequest, Rating otherRating)
    {
        // arrange
        var oldRating = CreateRating(RATING_ID, null, null);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(new List<Rating>() { otherRating }.ToImmutableList());

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateRating(RATING_ID, ratingRequest));

        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Never());
    }

    public static IEnumerable<object[]> GetTestData_Invalid()
    {
        // giving 10 points in category 1, 10 points has already been given in category 1 for another country
        yield return new object[] { CreateRatingRequest(10, 1, 2), CreateRating(1, 10, 2, 3) };
        // giving 12 points in category 2, 12 points has already been given in category 2 for another country
        yield return new object[] { CreateRatingRequest(4, 12, 2), CreateRating(1, 6, 12, 3) };
        // giving 12 points in category 3, 12 points has already been given in category 3 for another country
        yield return new object[] { CreateRatingRequest(4, 1, 12), CreateRating(1, 6, 3, 12) };
        // giving 9 points in category 1
        yield return new object[] { CreateRatingRequest(9, 1, 2), CreateRating(1, 4, 2, 3) };
        // giving 13 points in category 3
        yield return new object[] { CreateRatingRequest(3, 1, 13), CreateRating(1, 4, 2, 3) };
        // giving -1 points in category 2
        yield return new object[] { CreateRatingRequest(6, -1, 2), CreateRating(1, 4, 2, 3) };
        // giving 0 points in category 2
        yield return new object[] { CreateRatingRequest(6, 0, 2), CreateRating(1, 4, 2, 3) };
    }

    [Fact]
    public async void UpdateRating_RatingClosed()
    {
        // arrange
        var ratingRequest = CreateRatingRequest(1, 2, 3);
        _rateClosingServiceMock.Setup(m => m.ValidateRatingTime())
            .Throws<RatingIsClosedException>();

        // act and assert
        await Assert.ThrowsAsync<RatingIsClosedException>(async () => await _service.UpdateRating(RATING_ID, ratingRequest));

        _rateClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(It.IsAny<int>()), Times.Never());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(It.IsAny<int>()), Times.Never());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Never());
    }

    // helper methods

    private static RatingPointsRequestDto CreateRatingRequest
        (
        int category1Points, 
        int category2Points,
        int category3Points
        )
    {
        return new RatingPointsRequestDto
        {
            Category1Points = category1Points,
            Category2Points = category2Points,
            Category3Points = category3Points
        };
    }

    private static Rating CreateRating
        (
        int id,
        int category1Points,
        int category2Points,
        int category3Points,
        int pointsSum,
        int ranking
        )
    {
        return new Rating
        {
            Id = id,
            PlayerId = PLAYER_ID,
            Category1Points = category1Points,
            Category2Points = category2Points,
            Category3Points = category3Points,
            CountryId = 5678,
            PointsSum = pointsSum,
            Ranking = ranking,
            Country = CreateCountry(1000)
        };
    }

    private static Rating CreateRating
        (
        int id,
        int? pointsSum,
        int? ranking
        )
    {
        return CreateRating(id, pointsSum, ranking, 1000);
    }
    
    private static Rating CreateRating
        (
        int id,
        int? pointsSum,
        int? ranking, 
        int countryNumber
        )
    {
        return new Rating
        {
            Id = id,
            PlayerId = PLAYER_ID,
            PointsSum = pointsSum,
            Ranking = ranking,
            Category1Points = 1,
            Category2Points = 1,
            Category3Points = 1,
            CountryId = 5678,
            Country = CreateCountry(countryNumber)
        };
    }

    private static Country CreateCountry(int number)
    {
        return new Country
        { 
            Id = 23999,
            Name = "lk",
            Number = number
        };
    }
}