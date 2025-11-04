using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Api.Features.RatingClosing;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features.PlayerRatings;

public class PlayerRatingServiceTest
{
    private const int RATING_ID = 821;
    private const int PLAYER_ID = 657;

    private readonly Mock<IPlayerRatingRepository> _repositoryMock;
    private readonly Mock<IRatingClosingService> _ratingClosingServiceMock;
    private readonly Mock<ILogger<PlayerRatingService>> _loggerMock;
    private readonly PlayerRatingService _service;

    public PlayerRatingServiceTest()
    {
        _repositoryMock = new Mock<IPlayerRatingRepository>();
        _ratingClosingServiceMock = new Mock<IRatingClosingService>();
        _loggerMock = new Mock<ILogger<PlayerRatingService>>();

        _service = new PlayerRatingService(
            _repositoryMock.Object, 
            _ratingClosingServiceMock.Object,
            _loggerMock.Object);
    }

    // tests for getting all ratings
    [Fact]
    public async void GetAllPlayerRatings()
    {
        // arrange
        var expectedRatings = new List<PlayerRating>()
        {
            CreateInitialRating(1),
            CreateInitialRating(2),
        }.ToImmutableList();
        _repositoryMock.Setup(r => r.GetAllPlayerRatings())
            .ReturnsAsync(expectedRatings);

        // act
        var actualRatings = await _service.GetAllPlayerRatings();

        // assert
        Assert.Equal(expectedRatings, actualRatings);
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
        _repositoryMock.Verify(r => r.GetAllPlayerRatings(), Times.Once());
    }

    // tests for getting ratings by player id

    [Fact]
    public async void GetRatingsByPlayerId()
    {
        // arrange
        var rating1 = CreateRatingWithRankAndCountryNumber(1, null, 100);
        var rating2 = CreateRatingWithRankAndCountryNumber(2, 5, 5);
        var rating3 = CreateRatingWithRankAndCountryNumber(3, 7, 10);
        var rating4 = CreateRatingWithRankAndCountryNumber(4, 1, 19);
        var rating5 = CreateRatingWithRankAndCountryNumber(5, 5, 6);
        var rating6 = CreateRatingWithRankAndCountryNumber(6, null, 89);

        var ratings = new List<PlayerRating>() 
        { 
            rating1,
            rating2,
            rating3,
            rating4,
            rating5,
            rating6
        }.ToImmutableList();
        var expectedRatings = new List<PlayerRating>()
        {
            rating4,
            rating2, 
            rating5,
            rating3,
            rating6,
            rating1
        }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID))
            .ReturnsAsync(ratings);

        // act
        var actualRatings = await _service.GetPlayerRatingsByPlayerId(PLAYER_ID);

        // assert
        Assert.Equal(expectedRatings, actualRatings);

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID), Times.Once());
    }

    [Fact]
    public async void GetRatingsByPlayer_NoResults()
    {
        // arrange
        var ratings = new List<PlayerRating>() { }.ToImmutableList();

        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID))
            .ReturnsAsync(ratings);

        // act and assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _service.GetPlayerRatingsByPlayerId(PLAYER_ID)
        );
        
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Never());
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID), Times.Once());
    }

    // tests for updating rating points

    [Theory]
    [MemberData(nameof(GetTestData))]
    public async void UpdateRating_ValidOtherRating(
        RatingPointsRequestDto ratingRequest, 
        PlayerRating otherRating,
        PlayerRating expectedUpdatedRating
        )
    {
        // arrange
        var oldRating = CreateInitialRating(RATING_ID);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID))
            .ReturnsAsync(new List<PlayerRating>() { otherRating, oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<PlayerRating>()), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    public static IEnumerable<object[]> GetTestData()
    {
        var otherRatingId = 999;
        // giving 10 points in category 1, 10 points has already been given in category 3 for another rating
        yield return new object[] { 
            CreateRatingRequest(10, 1, 2), 
            CreateRatingWithPointsAndRank(otherRatingId, 6, 7, 10, 100, 1),
            CreateRatingWithPointsAndRank(RATING_ID, 10, 1, 2, 13, 2)
        };
        // giving 10 points in category 1, 12 points has already been given in category 1 for another rating
        yield return new object[] { 
            CreateRatingRequest(10, 1, 2), 
            CreateRatingWithPointsAndRank(otherRatingId, 12, 8, 4, 100, 1),
            CreateRatingWithPointsAndRank(RATING_ID, 10, 1, 2, 13, 2)
        };
        // giving 7 points in category 1, 7 points has already been given in category 1 for another rating
        yield return new object[] { 
            CreateRatingRequest(7, 1, 2), 
            CreateRatingWithPointsAndRank(otherRatingId, 7, 6, 4, 100, 1),
            CreateRatingWithPointsAndRank(RATING_ID, 7, 1, 2, 10, 2)
        };
    }

    // giving 10 points in category 2, 10 points has already been given in category 2 for the same country
    [Fact]
    public async void UpdateRating_ValidUpdatedRating()
    {
        // arrange
        var ratingRequest = CreateRatingRequest(1, 10, 2);
        var oldRating = CreateRatingWithPointsAndRank(RATING_ID, 1, 10, 5, 1000, 1);
        var expectedUpdatedRating = CreateRatingWithPointsAndRank(RATING_ID, 1, 10, 2, 13, 1);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID))
            .ReturnsAsync(new List<PlayerRating>() { oldRating }.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<PlayerRating>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    [Theory]
    [MemberData(nameof(GetTestDataRank))]
    public async void UpdateRating_CorrectRank(
        PlayerRating oldRating,
        List<PlayerRating> otherRatings,
        RatingPointsRequestDto ratingRequest,
        List<PlayerRating> expectedUpdatedRatings
        )
    {
        // arrange
        otherRatings.Add(oldRating);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID))
            .ReturnsAsync(otherRatings.ToImmutableList());

        // act
        await _service.UpdateRating(RATING_ID, ratingRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once);
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<PlayerRating>()), Times.Exactly(expectedUpdatedRatings.Count()));
        foreach (var expectedUpdatedRating in expectedUpdatedRatings)
        {
            _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
        }
    }

    public static IEnumerable<object[]> GetTestDataRank()
    {
        // 1
        // first rating, taking 1st place
        // moving all one down
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsSumAndRank(222, 32, 2),
                CreateRatingWithPointsSumAndRank(666, 25, 6),
                CreateRatingWithPointsSumAndRank(44455, 30, 5),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(22222, 32, 2)
            },
            CreateRatingRequest(12, 12, 12),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 12, 12, 12, 36, 1),
                CreateRatingWithPointsSumAndRank(111, 35, 2),
                CreateRatingWithPointsSumAndRank(222, 32, 3),
                CreateRatingWithPointsSumAndRank(22222, 32, 3),
                CreateRatingWithPointsSumAndRank(44455, 30, 6),
                CreateRatingWithPointsSumAndRank(444, 30, 5),
                CreateRatingWithPointsSumAndRank(666, 25, 7)
            }
        };
        // 2
        // first rating, taking 4th (last) place
        // no changes to the other's rank
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsSumAndRank(111, 32, 1),
                CreateRatingWithPointsSumAndRank(222, 25, 2),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(333, 20, 3)
            },
            CreateRatingRequest(1, 1, 1),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 1, 1, 1, 3, 4)
            }
        };
        // 3
        // first rating, taking shared 2nd place
        // resetting 2nd and moving all, but 1st and 2nd, one down
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsSumAndRank(222, 32, 2),
                CreateRatingWithPointsSumAndRank(666, 25, 6),
                CreateRatingWithPointsSumAndRank(44455, 30, 5),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(22222, 32, 2)
            },
            CreateRatingRequest(10, 10, 12),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 10, 10, 12, 32, 2),
                CreateRatingWithPointsSumAndRank(222, 32, 2),
                CreateRatingWithPointsSumAndRank(22222, 32, 2),
                CreateRatingWithPointsSumAndRank(44455, 30, 6),
                CreateRatingWithPointsSumAndRank(444, 30, 5),
                CreateRatingWithPointsSumAndRank(666, 25, 7)
            }
        };
        // 4
        // first rating, taking shared 4th place
        // resetting 4th and moving 6th one down
        yield return new object[] {
            CreateInitialRating(RATING_ID),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsSumAndRank(222, 32, 2),
                CreateRatingWithPointsSumAndRank(666, 25, 6),
                CreateRatingWithPointsSumAndRank(44455, 30, 5),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(22222, 32, 2)
        },
            CreateRatingRequest(10, 10, 10),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 10, 10, 10, 30, 4),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(44455, 30, 4),
                CreateRatingWithPointsSumAndRank(666, 25, 7)
            }
        };
        // 5
        // changing rating, from shared 4th (set to 5th) to 2nd
        // resetting 4th and moving 2nd, 3rd and 4th one down
        yield return new object[] {
            CreateRatingWithPointsSumAndRank(RATING_ID, 30, 5),
            new List<PlayerRating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(222, 32, 2),
                CreateRatingWithPointsSumAndRank(44466, 30, 6),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateRatingWithPointsSumAndRank(777, 20, 7),
                CreateRatingWithPointsSumAndRank(333, 31, 3)
            },
            CreateRatingRequest(12, 12, 10),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 12, 12, 10, 34, 2),
                CreateRatingWithPointsSumAndRank(222, 32, 3),
                CreateRatingWithPointsSumAndRank(333, 31, 4),
                CreateRatingWithPointsSumAndRank(444, 30, 5),
                CreateRatingWithPointsSumAndRank(44466, 30, 5)
            }
        };
        // 6
        // changing rating, from 2nd to shared 3rd
        // resetting 4th and moving 3rd and 4th one up
        yield return new object[] {
            CreateRatingWithPointsSumAndRank(RATING_ID, 33, 2),
            new List<PlayerRating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(333, 31, 3),
                CreateRatingWithPointsSumAndRank(44455, 30, 5),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateRatingWithPointsSumAndRank(666, 20, 6)
            },
            CreateRatingRequest(10, 10, 10),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 10, 10, 10, 30, 3),
                CreateRatingWithPointsSumAndRank(333, 31, 2),
                CreateRatingWithPointsSumAndRank(44455, 30, 3),
                CreateRatingWithPointsSumAndRank(444, 30, 3)
            }
        };
        // 7
        // changing rating, from 1st to 7th (last)
        // moving all one up
        yield return new object[] {
            CreateRatingWithPointsSumAndRank(RATING_ID, 36, 1),
            new List<PlayerRating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(222, 32, 2),
                CreateRatingWithPointsSumAndRank(22222, 32, 2),
                CreateRatingWithPointsSumAndRank(44455, 30, 5),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(44466, 30, 6),
                CreateRatingWithPointsSumAndRank(777, 20, 7)
            },
            CreateRatingRequest(1, 1, 1),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 1, 1, 1, 3, 7),
                CreateRatingWithPointsSumAndRank(222, 32, 1),
                CreateRatingWithPointsSumAndRank(22222, 32, 1),
                CreateRatingWithPointsSumAndRank(44455, 30, 4),
                CreateRatingWithPointsSumAndRank(444, 30, 3),
                CreateRatingWithPointsSumAndRank(44466, 30, 5),
                CreateRatingWithPointsSumAndRank(777, 20, 6)
            }
        };
        // 8
        // changing rating, points sum has decreased but no change in rank
        // no change in rank
        yield return new object[] {
            CreateRatingWithPointsSumAndRank(RATING_ID, 33, 3),
            new List<PlayerRating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateRatingWithPointsSumAndRank(222, 34, 2),
                CreateRatingWithPointsSumAndRank(44455, 30, 5),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(44466, 30, 6),
                CreateRatingWithPointsSumAndRank(777, 20, 7)
            },
            CreateRatingRequest(12, 10, 10),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 12, 10, 10, 32, 3)
            }
        };
        // 9
        // changing rating, points sum is unchanged
        // no change in rank
        yield return new object[] {
            CreateRatingWithPointsSumAndRank(RATING_ID, 32, 3),
            new List<PlayerRating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateRatingWithPointsSumAndRank(222, 34, 2),
                CreateRatingWithPointsSumAndRank(44455, 30, 5),
                CreateRatingWithPointsSumAndRank(444, 30, 4),
                CreateRatingWithPointsSumAndRank(44466, 30, 6),
                CreateRatingWithPointsSumAndRank(777, 20, 7)
            },
            CreateRatingRequest(12, 10, 10),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 12, 10, 10, 32, 3)
            }
        };
        // 10
        // changing rating, from shared 3rd place to not shared 3rd place
        // points sum has increased but not rank
        // should not share rank anymore, move other 3rd rating one down
        yield return new object[] {
            CreateRatingWithPointsSumAndRank(RATING_ID, 30, 3),
            new List<PlayerRating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateRatingWithPointsSumAndRank(222, 34, 2),
                CreateRatingWithPointsSumAndRank(333, 30, 3),
                CreateRatingWithPointsSumAndRank(555, 20, 5)
            },
            CreateRatingRequest(12, 10, 10),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 12, 10, 10, 32, 3),
                CreateRatingWithPointsSumAndRank(333, 30, 4)
            }
        };
        // 11
        // changing rating, from shared 3rd place to not shared 3rd place
        // points sum has decreased and so has rank
        // should not share rank anymore, move down and reset other 3rd ratings
        yield return new object[] {
            CreateRatingWithPointsSumAndRank(RATING_ID, 30, 3),
            new List<PlayerRating>()
            {
                CreateInitialRating(1000),
                CreateRatingWithPointsSumAndRank(111, 35, 1),
                CreateRatingWithPointsSumAndRank(222, 34, 2),
                CreateRatingWithPointsSumAndRank(333, 30, 3),
                CreateRatingWithPointsSumAndRank(666, 20, 6),
                CreateRatingWithPointsSumAndRank(33344, 30, 4)
            },
            CreateRatingRequest(8, 10, 10),
            new List<PlayerRating>()
            {
                CreateRatingWithPointsAndRank(RATING_ID, 8, 10, 10, 28, 5),
                CreateRatingWithPointsSumAndRank(333, 30, 3),
                CreateRatingWithPointsSumAndRank(33344, 30, 3)
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetTestData_Invalid))]
    public async void UpdateRating_Invalid(RatingPointsRequestDto ratingRequest, PlayerRating otherRating)
    {
        // arrange
        var oldRating = CreateRatingWithPointsSumAndRank(RATING_ID, null, null);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);
        _repositoryMock.Setup(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID))
            .ReturnsAsync(new List<PlayerRating>() { otherRating }.ToImmutableList());

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.UpdateRating(RATING_ID, ratingRequest)
            );

        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(PLAYER_ID), Times.Once());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<PlayerRating>()), Times.Never());
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
        _repositoryMock.Verify(r => r.GetPlayerRatingsByPlayerId(It.IsAny<int>()), Times.Never());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<PlayerRating>()), Times.Never());
    }

    // tests for updating rank

    [InlineData(12)]
    [InlineData(1)]
    [InlineData(26)]
    [Theory]
    public async void UpdateRatingRank_Valid(int rankRequest)
    {
        // arrange
        var oldRating = CreateRatingWithPointsSumAndRank(RATING_ID, 34, 15);
        var expectedUpdatedRating = CreateRatingWithPointsSumAndRank(RATING_ID, 34, rankRequest);

        _repositoryMock.Setup(m => m.GetRating(RATING_ID))
            .ReturnsAsync(oldRating);

        // act
        await _service.UpdatePlayerRating(RATING_ID, rankRequest);

        // assert
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(RATING_ID), Times.Once());

        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<PlayerRating>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateRating(expectedUpdatedRating), Times.Once());
    }

    [InlineData(-2)]
    [InlineData(0)]
    [InlineData(27)]
    [Theory]
    public async void UpdateRatingRank_Invalid(int rankRequest)
    {
        // arrange
        var oldRating = CreateRatingWithPointsSumAndRank(RATING_ID, 34, 15);
        var expectedUpdatedRating = CreateRatingWithPointsSumAndRank(RATING_ID, 34, rankRequest);

        // act and assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdatePlayerRating(RATING_ID, rankRequest));
 
        _ratingClosingServiceMock.Verify(m => m.ValidateRatingTime(), Times.Once());

        _repositoryMock.Verify(r => r.GetRating(It.IsAny<int>()), Times.Never());
        _repositoryMock.Verify(r => r.UpdateRating(It.IsAny<PlayerRating>()), Times.Never);
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

    private static PlayerRating CreateRatingWithPoints
        (
        int id,
        int category1Points,
        int category2Points,
        int category3Points
        )
    {
        return CreateRatingWithPointsAndRank
            (
            id, 
            category1Points,
            category2Points,
            category3Points,
            null, 
            null
            );
    }

    private static PlayerRating CreateRatingWithPointsAndRank
        (
        int id,
        int? category1Points,
        int? category2Points,
        int? category3Points,
        int? pointsSum,
        int? rank
        )
    {
        return CreateRating(
            id, 
            pointsSum,
            rank,
            category1Points,
            category2Points,
            category3Points,
            1000
            );
    }

    private static PlayerRating CreateInitialRating(int id)
    {
        return CreateRatingWithPointsSumAndRank(id, null, null);
    }

    private static PlayerRating CreateRatingWithPointsSumAndRank
        (
        int id,
        int? pointsSum,
        int? rank
        )
    {
        return CreateRatingWithPointsAndRank(
            id, 
            null,
            null,
            null,
            pointsSum,
            rank
            );
    }

    private static PlayerRating CreateRatingWithRankAndCountryNumber
        (
        int id,
        int? rank,
        int countryNumber
        )
    {
        return CreateRating(
            id,
            null,
            rank,
            null, 
            null, 
            null, 
            countryNumber
            );
    }

    private static PlayerRating CreateRating
        (
        int id,
        int? pointsSum,
        int? rank, 
        int? category1Points,
        int? category2Points,
        int? category3Points,
        int countryNumber
        )
    {
        return new PlayerRating
        {
            Id = id,
            PlayerId = PLAYER_ID,
            Prediction = CreatePrediciton(rank, pointsSum),
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

    private static Prediction CreatePrediciton(int? rank, int? pointsSum)
    {
        return new Prediction
        {
            Id = 34567,
            CalculatedRank = rank,
            TotalGivenPoints = pointsSum
        };
    }
}