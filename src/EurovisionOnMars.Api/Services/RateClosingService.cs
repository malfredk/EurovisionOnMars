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
    private readonly IConfiguration _configuration;
    private readonly ILogger<RateClosingService> _logger;

    public RateClosingService(
        IDateTimeNow dateTimeNow, 
        IConfiguration configuration,
        ILogger<RateClosingService> logger)
    {
        _dateTimeNow = dateTimeNow;
        _configuration = configuration;
        _logger = logger;
    }

    public void ValidateRatingTime()
    {
        var closingTime = GetClosingTime();
        var closingTimeOffset = new DateTimeOffset(closingTime, _dateTimeNow.OsloTimeZone.GetUtcOffset(closingTime));

        if (_dateTimeNow.Now > closingTimeOffset)
        {
            throw new RatingIsClosedException();
        }
    }

    private DateTime GetClosingTime()
    {
        var closingTimeString = _configuration.GetValue<string>("CLOSE_RATING_TIME");
        if (string.IsNullOrEmpty(closingTimeString))
        {
            throw new Exception("Unable to get close rating time");
        }
        var closingTime = DateTime.Parse(closingTimeString!);
        return closingTime;
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