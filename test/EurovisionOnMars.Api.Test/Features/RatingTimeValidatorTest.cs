using EurovisionOnMars.Api.Features;
using EurovisionOnMars.CustomException;
using Microsoft.Extensions.Logging;
using Moq;

namespace EurovisionOnMars.Api.Test.Features;

public class RatingTimeValidatorTest
{
    private readonly DateTimeOffset _ratingClosingTime = new DateTimeOffset(2024, 5, 11, 23, 50, 0, TimeSpan.Zero);
    private readonly Mock<IDateTimeNow> _dateTimeNowMock;
    private readonly Mock<ILogger<RatingTimeValidator>> _loggerMock;
    private readonly RatingTimeValidator _validator;
    
    // TODO 

    public RatingTimeValidatorTest()
    {
        _dateTimeNowMock = new Mock<IDateTimeNow>();
        _loggerMock = new Mock<ILogger<RatingTimeValidator>>();

        _validator = new RatingTimeValidator(
            _dateTimeNowMock.Object,
            _ratingClosingTime,
            _loggerMock.Object);
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void EnsureRatingIsOpen(DateTimeOffset dateTimeNow)
    {
        // arrange
        _dateTimeNowMock.Setup(m => m.Now)
            .Returns(dateTimeNow);

        // act
        _validator.EnsureRatingIsOpen();

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
    public void EnsureRatingIsOpen_Invalid(DateTimeOffset dateTimeNow)
    {
        // arrange
        _dateTimeNowMock.Setup(m => m.Now)
            .Returns(dateTimeNow);

        // act and assert
        Assert.Throws<RatingIsClosedException>(() => _validator.EnsureRatingIsOpen());

        _dateTimeNowMock.Verify(m => m.Now, Times.Once);
    }

    public static IEnumerable<object[]> GetTestData_Invalid()
    {
        yield return new object[] { new DateTimeOffset(2024, 5, 11, 23, 50, 01, TimeSpan.Zero) };
        yield return new object[] { new DateTimeOffset(2024, 5, 11, 23, 59, 00, TimeSpan.Zero) };
        yield return new object[] { new DateTimeOffset(2024, 5, 12, 00, 01, 00, TimeSpan.Zero) };
    }
}