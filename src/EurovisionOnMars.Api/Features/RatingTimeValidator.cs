using EurovisionOnMars.CustomException;

namespace EurovisionOnMars.Api.Features;

public interface IRatingTimeValidator
{
    public void EnsureRatingIsOpen();
}

public class RatingTimeValidator : IRatingTimeValidator
{
    public readonly IDateTimeNow _dateTimeNow;
    private readonly DateTimeOffset _ratingClosingTime;
    private readonly ILogger<RatingTimeValidator> _logger;

    public RatingTimeValidator(
        IDateTimeNow dateTimeNow, 
        DateTimeOffset ratingClosingTime,
        ILogger<RatingTimeValidator> logger)
    {
        _dateTimeNow = dateTimeNow;
        _ratingClosingTime = ratingClosingTime;
        _logger = logger;
    }

    public void EnsureRatingIsOpen()
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