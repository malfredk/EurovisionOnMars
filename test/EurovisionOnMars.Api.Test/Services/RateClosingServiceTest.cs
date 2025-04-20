using EurovisionOnMars.Api.Services;
using EurovisionOnMars.CustomException;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Services;

public class RateClosingServiceTest
{
    private readonly Mock<IDateTimeNow> _dateTimeNowMock;
    private readonly Mock<ILogger<RateClosingService>> _loggerMock;
    private readonly RateClosingService _service;

    public RateClosingServiceTest()
    {
        _dateTimeNowMock = new Mock<IDateTimeNow>();
        _loggerMock = new Mock<ILogger<RateClosingService>>();

        var inMemorySettings = new Dictionary<string, string> {
            {"CLOSE_RATING_TIME", "2024-05-11T23:50:00Z"}
        };
        IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _service = new RateClosingService(
            _dateTimeNowMock.Object,
            _configuration,
            _loggerMock.Object);
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void ValidateRatingTime(DateTime dateTimeNow)
    {
        // arrange
        _dateTimeNowMock.Setup(m => m.Now)
            .Returns(new DateTimeOffset(dateTimeNow));

        // act
        _service.ValidateRatingTime();

        // assert
        _dateTimeNowMock.Verify(m => m.Now, Times.Once);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { new DateTime(2024, 5, 11, 23, 50, 00, DateTimeKind.Utc) };
        yield return new object[] { new DateTime(2024, 5, 11, 21, 59, 00, DateTimeKind.Utc) };
        yield return new object[] { new DateTime(2024, 5, 10, 23, 59, 00, DateTimeKind.Utc) };
    }

    [Theory]
    [MemberData(nameof(GetTestData_Invalid))]
    public void ValidateRatingTime_Invalid(DateTime dateTimeNow)
    {
        // arrange
        _dateTimeNowMock.Setup(m => m.Now)
            .Returns(new DateTimeOffset(dateTimeNow));

        // act and assert
        Assert.Throws<RatingIsClosedException>(() => _service.ValidateRatingTime());

        _dateTimeNowMock.Verify(m => m.Now, Times.Once);
    }

    public static IEnumerable<object[]> GetTestData_Invalid()
    {
        yield return new object[] { new DateTime(2024, 5, 11, 23, 50, 01, DateTimeKind.Utc) };
        yield return new object[] { new DateTime(2024, 5, 11, 23, 59, 00, DateTimeKind.Utc) };
        yield return new object[] { new DateTime(2024, 5, 12, 00, 01, 00, DateTimeKind.Utc) };
    }
}