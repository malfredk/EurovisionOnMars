using EurovisionOnMars.CustomException;

namespace EurovisionOnMars.Api.Services;

public interface IRatingClosingService
{
    public void ValidateRatingTime();
}

public class RatingClosingService : IRatingClosingService
{
    public readonly IDateTimeNow _dateTimeNow;
    private readonly DateTimeOffset _ratingClosingTime;
    private readonly ILogger<RatingClosingService> _logger;

    public RatingClosingService(
        IDateTimeNow dateTimeNow, 
        DateTimeOffset ratingClosingTime,
        ILogger<RatingClosingService> logger)
    {
        _dateTimeNow = dateTimeNow;
        _ratingClosingTime = ratingClosingTime;
        _logger = logger;
    }

    public void ValidateRatingTime()
    {        
        if (_dateTimeNow.Now > _ratingClosingTime)
        {
            throw new RatingIsClosedException();
        }
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