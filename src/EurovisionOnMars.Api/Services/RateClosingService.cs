using EurovisionOnMars.CustomException;
using TimeZoneConverter;

namespace EurovisionOnMars.Api.Services;

public interface IRateClosingService
{
    public void ValidateRatingTime();
}

public class RateClosingService : IRateClosingService
{
    public readonly IDateTimeNow _dateTimeNow;
    private readonly ILogger<RateClosingService> _logger;

    public RateClosingService(IDateTimeNow dateTimeNow, ILogger<RateClosingService> logger)
    {
        _dateTimeNow = dateTimeNow;
        _logger = logger;
    }

    public void ValidateRatingTime()
    {
        var closingTime = new DateTime(2024, 5, 11, 23, 50, 00);
        var closingTimeOffset = new DateTimeOffset(closingTime, _dateTimeNow.OsloTimeZone.GetUtcOffset(closingTime));

        if (_dateTimeNow.Now > closingTimeOffset)
        {
            throw new RatingIsClosedException();
        }
    }
}

public interface IDateTimeNow
{
    public DateTimeOffset Now { get; }
    public TimeZoneInfo OsloTimeZone { get; }
}

public class DateTimeNow : IDateTimeNow
{
    public TimeZoneInfo OsloTimeZone => TZConvert.GetTimeZoneInfo("Europe/Oslo");

    public DateTimeOffset Now => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, OsloTimeZone);
}