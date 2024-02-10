namespace EurovisionOnMars.CustomException;

public class DuplicateUsernameException : ArgumentException
{
    public DuplicateUsernameException()
    {
    }

    public DuplicateUsernameException(string message)
        : base(message)
    {
    }

    public DuplicateUsernameException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
