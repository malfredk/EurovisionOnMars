using EurovisionOnMars.CustomException;
using System.Globalization;

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
        if (!GetClosingTime(out DateTimeOffset closingTime))
        {
            throw new FormatException("Invalid format of CLOSE_RATING_TIME");
        }

        if (_dateTimeNow.Now > closingTime)
        {
            throw new RatingIsClosedException();
        }
    }

    private bool GetClosingTime(out DateTimeOffset result)
    {
        var closingTimeString = _configuration.GetValue<string>("CLOSE_RATING_TIME");

        if (string.IsNullOrEmpty(closingTimeString))
        {
            throw new Exception("Unable to get close rating time");
        }

        string[] validFormats = {
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:sszzz"
        };
        return DateTimeOffset.TryParseExact(
            closingTimeString,
            validFormats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out result
        );
    }
}

public interface IDateTimeNow
{
    public DateTimeOffset Now { get; }
}

public class DateTimeNow : IDateTimeNow
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}