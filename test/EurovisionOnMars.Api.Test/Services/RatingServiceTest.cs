using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using SQLitePCL;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Services;

public class RatingServiceTest
{
    private readonly Mock<IRatingRepository> _repositoryMock;
    private readonly Mock<ILogger<RatingService>> _loggerMock;
    private readonly RatingService _service;

    public RatingServiceTest()
    {
        _repositoryMock = new Mock<IRatingRepository>();
        _loggerMock = new Mock<ILogger<RatingService>>();

        _service = new RatingService(_repositoryMock.Object, _loggerMock.Object);
    }

    // tests for getting ratings by player

    [Fact]
    public async void GetRatingsByPlayer()
    {
        // arrange
        var playerId = 788778;

        var rating1 = CreateRating(1, null, null);
        var rating2 = CreateRating(2, null, 5);
        var rating3 = CreateRating(3, null, 7);
        var rating4 = CreateRating(4, null, 1);
        var rating5 = CreateRating(5, null, 5);
        var rating6 = CreateRating(6, null, null);

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
            rating1,
            rating6
        }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(playerId))
            .ReturnsAsync(expectedRatings);

        // act
        var ratings = await _service.GetRatingsByPlayer(playerId);

        // assert
        Assert.Equal(ratings, sortedExpectedRatings);

        _repositoryMock.Verify(r => r.GetRatingsByPlayer(playerId), Times.Once());
    }

    [Fact]
    public async void GetRatingsByPlayer_NoResults()
    {
        // arrange
        var playerId = 657;

        var expectedRatings = new List<Rating>() { }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(playerId))
            .ReturnsAsync(expectedRatings);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetRatingsByPlayer(playerId));

        _repositoryMock.Verify(r => r.GetRatingsByPlayer(playerId), Times.Once());
    }

    // tests for getting a rating by id

    [Fact]
    public async void GetRating()
    {
        // arrange
        var id = 765;

        var expectedRating = CreateRating(12, 1, null, 3);

        _repositoryMock.Setup(r => r.GetRating(id))
            .ReturnsAsync(expectedRating);

        // act
        var rating = await _service.GetRating(id);

        // assert
        Assert.Equal(rating, expectedRating);

        _repositoryMock.Verify(r => r.GetRating(id), Times.Once());
    }

    [Fact]
    public async void GetRating_InvalidId()
    {
        // arrange
        var id = 3232;

        var expectedRating = (Rating)null;

        _repositoryMock.Setup(r => r.GetRating(id))
            .ReturnsAsync(expectedRating);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetRating(id));

        _repositoryMock.Verify(r => r.GetRating(id), Times.Once());
    }

    // tests for updating rating

    [Theory]
    [MemberData(nameof(GetTestData))]
    public async void UpdateRating_ValidOtherRating(Rating newRating, Rating otherRating)
    {
        // arrange
        var playerId = 657;
        var oldRating = CreateRating(100, null, null, null);

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(playerId))
            .ReturnsAsync(new List<Rating>() { otherRating, oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(newRating);

        // assert
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(playerId), Times.Once());

        _repositoryMock.Verify(r  => r.UpdateRating(It.IsAny<Rating>()), Times.Exactly(2));
        _repositoryMock.Verify(r => r.UpdateRating(otherRating), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(newRating), Times.Once());
    }

    public static IEnumerable<object[]> GetTestData()
    {
        // giving 10 points in category 1, 10 points has already been given in category 3 for another country
        yield return new object[] { CreateRating(100, 10, 1, 2), CreateRating(1, 6, 2, 10) };
        // giving 10 points in category 1, 12 points has already been given in category 1 for another country
        yield return new object[] { CreateRating(100, 10, 1, 2), CreateRating(1, 12, 2, 5) };
        // giving 7 points in category 1, 7 points has already been given in category 1 for another country
        yield return new object[] { CreateRating(100, 7, 1, 2), CreateRating(1, 7, 3, 6) };
    }

    // giving 10 points in category 2, 10 points has already been given in category 2 for the same country
    [Fact]
    public async void UpdateRating_ValidUpdatedRating()
    {
        // arrange
        var playerId = 657;
        var oldRating = CreateRating(100, 10, 5, 8);
        var newRating = CreateRating(100, 10, 1, 2);

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(playerId))
            .ReturnsAsync(new List<Rating>() { oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(newRating);

        // assert
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(playerId), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateRating(newRating), Times.Once());
    }

    [Fact]
    public async void UpdateRating_Sorting()
    {
        // arrange
        var playerId = 657;

        var oldRating = CreateRating(100, null, null);
        var newRating = CreateRating(100, 500, 1);
        var expectedRating = CreateRating(100, 500, 4);

        var rating1 = CreateRating(1, 1000, null);
        var expectedRating1 = CreateRating(1, 1000, 1);

        var rating2 = CreateRating(2, 1000, null);
        var expectedRating2 = CreateRating(2, 1000, 1);

        var rating3 = CreateRating(3, null, null);
        var expectedRating3 = CreateRating(3, null, null);

        var rating4 = CreateRating(4, 700, null);
        var expectedRating4 = CreateRating(4, 700, 3);

        var rating5 = CreateRating(5, 500, 2);
        var expectedRating5 = CreateRating(5, 500, 4);

        var rating6 = CreateRating(6, 300, 10);
        var expectedRating6 = CreateRating(6, 300, 6);

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(playerId))
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
        await _service.UpdateRating(newRating);

        // assert
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(playerId), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Exactly(7));
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating1), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating2), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating3), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating4), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating5), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedRating6), Times.Once());
    }

    [Theory]
    [MemberData(nameof(GetTestData_Invalid))]
    public async void UpdateRating_Invalid(Rating updatedRating, Rating existingRating)
    {
        // arrange
        var playerId = 657;

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(playerId))
            .ReturnsAsync(new List<Rating>() { existingRating }.ToImmutableList());

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateRating(updatedRating));

        _repositoryMock.Verify(r => r.GetRatingsByPlayer(playerId), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Never());
    }

    public static IEnumerable<object[]> GetTestData_Invalid()
    {
        // giving 10 points in category 1, 10 points has already been given in category 1 for another country
        yield return new object[] { CreateRating(100, 10, 1, 2), CreateRating(1, 10, 2, 3) };
        // giving 12 points in category 2, 12 points has already been given in category 2 for another country
        yield return new object[] { CreateRating(100, 4, 12, 2), CreateRating(1, 6, 12, 3) };
        // giving 12 points in category 3, 12 points has already been given in category 3 for another country
        yield return new object[] { CreateRating(100, 4, 1, 12), CreateRating(1, 6, 3, 12) };
        // giving 9 points in category 1
        yield return new object[] { CreateRating(100, 9, 1, 2), CreateRating(1, 4, 2, 3) };
        // giving null points in category 1
        yield return new object[] { CreateRating(100, null, 1, 2), CreateRating(1, 4, 2, 3) };
        // giving 13 points in category 3
        yield return new object[] { CreateRating(100, 3, 1, 13), CreateRating(1, 4, 2, 3) };
        // giving -1 points in category 2
        yield return new object[] { CreateRating(100, 6, -1, 2), CreateRating(1, 4, 2, 3) };
        // giving 0 points in category 2
        yield return new object[] { CreateRating(100, 6, 0, 2), CreateRating(1, 4, 2, 3) };
    }

    // helper methods

    private static Rating CreateRating
        (
        int id, 
        int? category1Points, 
        int? category2Points,
        int? category3Points
        )
    {
        return new Rating
        {
            Id = id,
            PlayerId = 657,
            Category1Points = category1Points,
            Category2Points = category2Points,
            Category3Points = category3Points,
            CountryId = 5678
        };
    }

    private static Rating CreateRating
        (
        int id,
        int? pointsSum,
        int? ranking
        )
    {
        return new Rating
        {
            Id = id,
            PlayerId = 657,
            PointsSum = pointsSum,
            Ranking = ranking,
            Category1Points = 1,
            Category2Points = 1,
            Category3Points = 1,
            CountryId = 5678
        };
    }
}
