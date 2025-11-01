using EurovisionOnMars.Api.Features.RatingClosing;
using EurovisionOnMars.CustomException;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Services;

public class RatingClosingServiceTest
{
    private readonly DateTimeOffset _ratingClosingTime = new DateTimeOffset(2024, 5, 11, 23, 50, 0, TimeSpan.Zero);
    private readonly Mock<IDateTimeNow> _dateTimeNowMock;
    private readonly Mock<ILogger<RatingClosingService>> _loggerMock;
    private readonly RatingClosingService _service;

    public RatingClosingServiceTest()
    {
        _dateTimeNowMock = new Mock<IDateTimeNow>();
        _loggerMock = new Mock<ILogger<RatingClosingService>>();

        _service = new RatingClosingService(
            _dateTimeNowMock.Object,
            _ratingClosingTime,
            _loggerMock.Object);
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void ValidateRatingTime(DateTimeOffset dateTimeNow)
    {
        // arrange
        _dateTimeNowMock.Setup(m => m.Now)
            .Returns(dateTimeNow);

        // act
        _service.ValidateRatingTime();

        // assert
        _dateTimeNowMock.Verify(m => m.Now, Times.Once);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { new DateTimeOffset(2024, 5, 11, 23, 50, 00, TimeSpan.Zero) };
        yield return new object[] { new DateTimeOffset(2024, 5, 11, 21, 59, 00, TimeSpan.Zero) };
        yield return new object[] { new DateTimeOffset(2024, 5, 10, 23, 59, 00, TimeSpan.Zero) };
    }

    [Theory]
    [MemberData(nameof(GetTestData_Invalid))]
    public void ValidateRatingTime_Invalid(DateTimeOffset dateTimeNow)
    {
        // arrange
        _dateTimeNowMock.Setup(m => m.Now)
            .Returns(dateTimeNow);

        // act and assert
        Assert.Throws<RatingIsClosedException>(() => _service.ValidateRatingTime());

        _dateTimeNowMock.Verify(m => m.Now, Times.Once);
    }

    public static IEnumerable<object[]> GetTestData_Invalid()
    {
        yield return new object[] { new DateTimeOffset(2024, 5, 11, 23, 50, 01, TimeSpan.Zero) };
        yield return new object[] { new DateTimeOffset(2024, 5, 11, 23, 59, 00, TimeSpan.Zero) };
        yield return new object[] { new DateTimeOffset(2024, 5, 12, 00, 01, 00, TimeSpan.Zero) };
    }
}