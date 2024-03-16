namespace EurovisionOnMars.CustomException;

public class RatingIsClosedException : Exception
{
    public RatingIsClosedException()
    {
    }

    public RatingIsClosedException(string message)
        : base(message)
    {
    }

    public RatingIsClosedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}