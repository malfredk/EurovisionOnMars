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
    private readonly Mock<IRatingClosingService> _ratingClosingServiceMock;
    private readonly Mock<ILogger<RatingService>> _loggerMock;
    private readonly RatingService _service;

    public RatingServiceTest()
    {
        _repositoryMock = new Mock<IRatingRepository>();
        _ratingClosingServiceMock = new Mock<IRatingClosingService>();
        _loggerMock = new Mock<ILogger<RatingService>>();

        _service = new RatingService(
            _repositoryMock.Object, 
            _ratingClosingServiceMock.Object,
            _loggerMock.Object);
    }

    // tests for getting ratings by player

    [Fact]
    public async void GetRatingsByPlayer()
    {
        // arrange
        var rating1 = CreateRatingForSorting(1, null, 100);
        var rating2 = CreateRatingForSorting(2, 5, 5);
        var rating3 = CreateRatingForSorting(3, 7, 10);
        var rating4 = CreateRatingForSorting(4, 1, 19);
        var rating5 = CreateRatingForSorting(5, 5, 6);
        var rating6 = CreateRatingForSorting(6, null, 89);

        var ratings = new List<Rating>() 
        { 
            rating1,
            rating2,
            rating3,
            rating4,
            rating5,
            rating6
        }.ToImmutableList();
        var expectedSortedRatings = new List<Rating>()
        {
            rating4,
            rating2, 
            rating5,
            rating3,
            rating6,
            rating1
        }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(ratings);

        // act
        var actualRatings = await _service.GetRatingsByPlayer(PLAYER_ID);

        // assert
        Assert.Equal(expectedSortedRatings, actualRatings);

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());
    }

    [Fact]
    public async void GetRatingsByPlayer_NoResults()
    {
        // arrange
        var ratings = new List<Rating>() { }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(ratings);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetRatingsByPlayer(PLAYER_ID)
            );
        
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());
    }

    // tests for getting a rating by id

    [Fact]
    public async void GetRating()
    {
        // arrange
        var expectedRating = CreateInitialRating(RATING_ID);

        _repositoryMock.Setup(r => r.GetRating(RATING_ID))
            .ReturnsAsync(expectedRating);

        // act
        var actualRating = await _service.GetRating(RATING_ID);

        // assert
        Assert.Equal(actualRating, expectedRating);

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
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
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetRating(RATING_ID)
            );

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
    }

    // tests for updating rating points

    [Theory]
    [MemberData(nameof(GetTestData))]
    public async void UpdateRating_ValidOtherRating(
        RatingPointsRequestDto ratingRequest, 
        Rating otherRating,
        Rating expectedUpdatedRating
        )
    {
        // arrange
        var oldRating = CreateInitialRating(RATING_ID);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(new List<Rating>() { otherRating, oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    public static IEnumerable<object[]> GetTestData()
    {
        var otherRatingId = 999;
        // giving 10 points in category 1, 10 points has already been given in category 3 for another rating
        yield return new object[] { 
            CreateRatingRequest(10, 1, 2), 
            CreateRatingWithPointsAndRanking(otherRatingId, 6, 7, 10, 100, 1),
            CreateRatingWithPointsAndRanking(RATING_ID, 10, 1, 2, 13, 2)
        };
        // giving 10 points in category 1, 12 points has already been given in category 1 for another rating
        yield return new object[] { 
            CreateRatingRequest(10, 1, 2), 
            CreateRatingWithPointsAndRanking(otherRatingId, 12, 8, 4, 100, 1),
            CreateRatingWithPointsAndRanking(RATING_ID, 10, 1, 2, 13, 2)
        };
        // giving 7 points in category 1, 7 points has already been given in category 1 for another rating
        yield return new object[] { 
            CreateRatingRequest(7, 1, 2), 
            CreateRatingWithPointsAndRanking(otherRatingId, 7, 6, 4, 100, 1),
            CreateRatingWithPointsAndRanking(RATING_ID, 7, 1, 2, 10, 2)
        };
    }

    // giving 10 points in category 2, 10 points has already been given in category 2 for the same country
    [Fact]
    public async void UpdateRating_ValidUpdatedRating()
    {
        // arrange
        var ratingRequest = CreateRatingRequest(1, 10, 2);
        var oldRating = CreateRatingWithPointsAndRanking(RATING_ID, 1, 10, 5, 1000, 1);
        var expectedUpdatedRating = CreateRatingWithPointsAndRanking(RATING_ID, 1, 10, 2, 13, 1);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(new List<Rating>() { oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    [Theory]
    [MemberData(nameof(GetTestDataRanking))]
    public async void UpdateRating_CorrectRanking(
        Rating oldRating,
        List<Rating> otherRatings,
        RatingPointsRequestDto ratingRequest,
        List<Rating> expectedUpdatedRatings
        )
    {
        // arrange
        otherRatings.Add(oldRating);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(otherRatings.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once);
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Exactly(expectedUpdatedRatings.Count()));
        foreach (var expectedUpdatedRating in expectedUpdatedRatings)
        {
            _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
        }
    }

    public static IEnumerable<object[]> GetTestDataRanking()
    {
        // 1
        // first rating, taking 1st place
        // moving all one down
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<Rating>()
            {
                CreateRatingWithPointsSumAndRanking(222, 32, 2),
                CreateRatingWithPointsSumAndRanking(666, 25, 6),
                CreateRatingWithPointsSumAndRanking(44455, 30, 5),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(22222, 32, 2)
            },
            CreateRatingRequest(12, 12, 12),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 12, 12, 12, 36, 1),
                CreateRatingWithPointsSumAndRanking(111, 35, 2),
                CreateRatingWithPointsSumAndRanking(222, 32, 3),
                CreateRatingWithPointsSumAndRanking(22222, 32, 3),
                CreateRatingWithPointsSumAndRanking(44455, 30, 6),
                CreateRatingWithPointsSumAndRanking(444, 30, 5),
                CreateRatingWithPointsSumAndRanking(666, 25, 7)
            }
        };
        // 2
        // first rating, taking 4th (last) place
        // no changes to the other's ranking
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<Rating>()
            {
                CreateRatingWithPointsSumAndRanking(111, 32, 1),
                CreateRatingWithPointsSumAndRanking(222, 25, 2),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(333, 20, 3)
            },
            CreateRatingRequest(1, 1, 1),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 1, 1, 1, 3, 4)
            }
        };
        // 3
        // first rating, taking shared 2nd place
        // resetting 2nd and moving all, but 1st and 2nd, one down
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<Rating>()
            {
                CreateRatingWithPointsSumAndRanking(222, 32, 2),
                CreateRatingWithPointsSumAndRanking(666, 25, 6),
                CreateRatingWithPointsSumAndRanking(44455, 30, 5),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(22222, 32, 2)
            },
            CreateRatingRequest(10, 10, 12),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 10, 10, 12, 32, 2),
                CreateRatingWithPointsSumAndRanking(222, 32, 2),
                CreateRatingWithPointsSumAndRanking(22222, 32, 2),
                CreateRatingWithPointsSumAndRanking(44455, 30, 6),
                CreateRatingWithPointsSumAndRanking(444, 30, 5),
                CreateRatingWithPointsSumAndRanking(666, 25, 7)
            }
        };
        // 4
        // first rating, taking shared 4th place
        // resetting 4th and moving 6th one down
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<Rating>()
            {
                CreateRatingWithPointsSumAndRanking(222, 32, 2),
                CreateRatingWithPointsSumAndRanking(666, 25, 6),
                CreateRatingWithPointsSumAndRanking(44455, 30, 5),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(22222, 32, 2)
        },
            CreateRatingRequest(10, 10, 10),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 10, 10, 10, 30, 4),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(44455, 30, 4),
                CreateRatingWithPointsSumAndRanking(666, 25, 7)
            }
        };
        // 5
        // changing rating, from shared 4th (set to 5th) to 2nd
        // resetting 4th and moving 2nd, 3rd and 4th one down
        yield return new object[] {
            CreateRatingWithPointsSumAndRanking(RATING_ID, 30, 5),
            new List<Rating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(222, 32, 2),
                CreateRatingWithPointsSumAndRanking(44466, 30, 6),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateRatingWithPointsSumAndRanking(777, 20, 7),
                CreateRatingWithPointsSumAndRanking(333, 31, 3)
            },
            CreateRatingRequest(12, 12, 10),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 12, 12, 10, 34, 2),
                CreateRatingWithPointsSumAndRanking(222, 32, 3),
                CreateRatingWithPointsSumAndRanking(333, 31, 4),
                CreateRatingWithPointsSumAndRanking(444, 30, 5),
                CreateRatingWithPointsSumAndRanking(44466, 30, 5)
            }
        };
        // 6
        // changing rating, from 2nd to shared 3rd
        // resetting 4th and moving 3rd and 4th one up
        yield return new object[] {
            CreateRatingWithPointsSumAndRanking(RATING_ID, 33, 2),
            new List<Rating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(333, 31, 3),
                CreateRatingWithPointsSumAndRanking(44455, 30, 5),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateRatingWithPointsSumAndRanking(666, 20, 6)
            },
            CreateRatingRequest(10, 10, 10),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 10, 10, 10, 30, 3),
                CreateRatingWithPointsSumAndRanking(333, 31, 2),
                CreateRatingWithPointsSumAndRanking(44455, 30, 3),
                CreateRatingWithPointsSumAndRanking(444, 30, 3)
            }
        };
        // 7
        // changing rating, from 1st to 7th (last)
        // moving all one up
        yield return new object[] {
            CreateRatingWithPointsSumAndRanking(RATING_ID, 36, 1),
            new List<Rating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(222, 32, 2),
                CreateRatingWithPointsSumAndRanking(22222, 32, 2),
                CreateRatingWithPointsSumAndRanking(44455, 30, 5),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(44466, 30, 6),
                CreateRatingWithPointsSumAndRanking(777, 20, 7)
            },
            CreateRatingRequest(1, 1, 1),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 1, 1, 1, 3, 7),
                CreateRatingWithPointsSumAndRanking(222, 32, 1),
                CreateRatingWithPointsSumAndRanking(22222, 32, 1),
                CreateRatingWithPointsSumAndRanking(44455, 30, 4),
                CreateRatingWithPointsSumAndRanking(444, 30, 3),
                CreateRatingWithPointsSumAndRanking(44466, 30, 5),
                CreateRatingWithPointsSumAndRanking(777, 20, 6)
            }
        };
        // 8
        // changing rating, points sum has decreased but no change in ranking
        // no change in ranking
        yield return new object[] {
            CreateRatingWithPointsSumAndRanking(RATING_ID, 33, 3),
            new List<Rating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateRatingWithPointsSumAndRanking(222, 34, 2),
                CreateRatingWithPointsSumAndRanking(44455, 30, 5),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(44466, 30, 6),
                CreateRatingWithPointsSumAndRanking(777, 20, 7)
            },
            CreateRatingRequest(12, 10, 10),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 12, 10, 10, 32, 3)
            }
        };
        // 9
        // changing rating, points sum is unchanged
        // no change in ranking
        yield return new object[] {
            CreateRatingWithPointsSumAndRanking(RATING_ID, 32, 3),
            new List<Rating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateRatingWithPointsSumAndRanking(222, 34, 2),
                CreateRatingWithPointsSumAndRanking(44455, 30, 5),
                CreateRatingWithPointsSumAndRanking(444, 30, 4),
                CreateRatingWithPointsSumAndRanking(44466, 30, 6),
                CreateRatingWithPointsSumAndRanking(777, 20, 7)
            },
            CreateRatingRequest(12, 10, 10),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 12, 10, 10, 32, 3)
            }
        };
        // 10
        // changing rating, from shared 3rd place to not shared 3rd place
        // points sum has increased but not ranking
        // should not share ranking anymore, move other 3rd rating one down
        yield return new object[] {
            CreateRatingWithPointsSumAndRanking(RATING_ID, 30, 3),
            new List<Rating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateRatingWithPointsSumAndRanking(222, 34, 2),
                CreateRatingWithPointsSumAndRanking(333, 30, 3),
                CreateRatingWithPointsSumAndRanking(555, 20, 5)
            },
            CreateRatingRequest(12, 10, 10),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 12, 10, 10, 32, 3),
                CreateRatingWithPointsSumAndRanking(333, 30, 4)
            }
        };
        // 11
        // changing rating, from shared 3rd place to not shared 3rd place
        // points sum has decreased and so has ranking
        // should not share ranking anymore, move down and reset other 3rd ratings
        yield return new object[] {
            CreateRatingWithPointsSumAndRanking(RATING_ID, 30, 3),
            new List<Rating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRanking(111, 35, 1),
                CreateRatingWithPointsSumAndRanking(222, 34, 2),
                CreateRatingWithPointsSumAndRanking(333, 30, 3),
                CreateRatingWithPointsSumAndRanking(666, 20, 6),
                CreateRatingWithPointsSumAndRanking(33344, 30, 4)
            },
            CreateRatingRequest(8, 10, 10),
            new List<Rating>()
            {
                CreateRatingWithPointsAndRanking(RATING_ID, 8, 10, 10, 28, 5),
                CreateRatingWithPointsSumAndRanking(333, 30, 3),
                CreateRatingWithPointsSumAndRanking(33344, 30, 3)
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetTestData_Invalid))]
    public async void UpdateRating_Invalid(RatingPointsRequestDto ratingRequest, Rating otherRating)
    {
        // arrange
        var oldRating = CreateRatingWithPointsSumAndRanking(RATING_ID, null, null);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetRatingsByPlayer(PLAYER_ID))
            .ReturnsAsync(new List<Rating>() { otherRating }.ToImmutableList());

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.UpdateRating(RATING_ID, ratingRequest)
            );

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(PLAYER_ID), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Never());
    }

    public static IEnumerable<object[]> GetTestData_Invalid()
    {
        var otherRatingId = 999;
        var simpleOtherRating = CreateInitialRating(otherRatingId);
        // giving 10 points in category 1, 10 points has already been given in category 1 for another rating
        yield return new object[] 
        { 
            CreateRatingRequest(10, 1, 2), 
            CreateRatingWithPoints(otherRatingId, 10, 3, 4) 
        };
        // giving 12 points in category 2, 12 points has already been given in category 2 for another rating
        yield return new object[] 
        { 
            CreateRatingRequest(4, 12, 2), 
            CreateRatingWithPoints(otherRatingId, 6, 12, 3) 
        };
        // giving 12 points in category 3, 12 points has already been given in category 3 for another rating
        yield return new object[] 
        { 
            CreateRatingRequest(4, 1, 12), 
            CreateRatingWithPoints(otherRatingId, 1, 6, 12) };
        // giving 9 points in category 1
        yield return new object[] 
        { 
            CreateRatingRequest(9, 1, 2),
            simpleOtherRating
        };
        // giving 13 points in category 3
        yield return new object[] 
        { 
            CreateRatingRequest(3, 1, 13),
            simpleOtherRating
        };
        // giving -1 points in category 2
        yield return new object[] 
        { 
            CreateRatingRequest(6, -1, 2),
            simpleOtherRating
        };
        // giving 0 points in category 2
        yield return new object[] 
        { 
            CreateRatingRequest(6, 0, 2),
            simpleOtherRating
        };
    }

    [Fact]
    public async void UpdateRating_RatingClosed()
    {
        // arrange
        var ratingRequest = CreateRatingRequest(1, 2, 3);
        _ratingClosingServiceMock.Setup(m => m.ValidateRatingTime())
            .Throws<RatingIsClosedException>();

        // act and assert
        await Assert.ThrowsAsync<RatingIsClosedException>(
            async () => await _service.UpdateRating(RATING_ID, ratingRequest)
            );

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(It.IsAny<int>()), Times.Never());
        _repositoryMock.Verify(r => r.GetRatingsByPlayer(It.IsAny<int>()), Times.Never());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Never());
    }

    // tests for updating ranking

    [InlineData(12)]
    [InlineData(1)]
    [InlineData(26)]
    [Theory]
    public async void UpdateRatingRanking_Valid(int rankingRequest)
    {
        // arrange
        var oldRating = CreateRatingWithPointsSumAndRanking(RATING_ID, 34, 15);
        var expectedUpdatedRating = CreateRatingWithPointsSumAndRanking(RATING_ID, 34, rankingRequest);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);

        // act
        await _service.UpdateRating(RATING_ID, rankingRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(27)]
    [Theory]
    public async void UpdateRatingRanking_Invalid(int rankingRequest)
    {
        // arrange
        var oldRating = CreateRatingWithPointsSumAndRanking(RATING_ID, 34, 15);
        var expectedUpdatedRating = CreateRatingWithPointsSumAndRanking(RATING_ID, 34, rankingRequest);

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateRating(RATING_ID, rankingRequest));
 
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(It.IsAny<int>()), Times.Never());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<Rating>()), Times.Never);
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

    private static Rating CreateRatingWithPoints
        (
        int id,
        int category1Points,
        int category2Points,
        int category3Points
        )
    {
        return CreateRatingWithPointsAndRanking
            (
            id, 
            category1Points,
            category2Points,
            category3Points,
            null, 
            null
            );
    }

    private static Rating CreateRatingWithPointsAndRanking
        (
        int id,
        int? category1Points,
        int? category2Points,
        int? category3Points,
        int? pointsSum,
        int? ranking
        )
    {
        return CreateRating(
            id, 
            pointsSum,
            ranking,
            category1Points,
            category2Points,
            category3Points,
            1000
            );
    }

    private static Rating CreateInitialRating(int id)
    {
        return CreateRatingWithPointsSumAndRanking(id, null, null);
    }

    private static Rating CreateRatingWithPointsSumAndRanking
        (
        int id,
        int? pointsSum,
        int? ranking
        )
    {
        return CreateRatingWithPointsAndRanking(
            id, 
            null,
            null,
            null,
            pointsSum,
            ranking
            );
    }

    private static Rating CreateRatingForSorting
        (
        int id,
        int? ranking,
        int countryNumber
        )
    {
        return CreateRating(
            id,
            null,
            ranking,
            null, 
            null, 
            null, 
            countryNumber
            );
    }

    private static Rating CreateRating
        (
        int id,
        int? pointsSum,
        int? ranking, 
        int? category1Points,
        int? category2Points,
        int? category3Points,
        int countryNumber
        )
    {
        return new Rating
        {
            Id = id,
            PlayerId = PLAYER_ID,
            PointsSum = pointsSum,
            Ranking = ranking,
            Category1Points = category1Points,
            Category2Points = category2Points,
            Category3Points = category3Points,
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