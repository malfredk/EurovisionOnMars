using System.Globalization;

namespace EurovisionOnMars.Api.Configurations;

public static class ConfigurationValidator
{
    public static DateTimeOffset GetAndValidateRatingClosingTime(IConfiguration configuration)
    {
        var ratingClosingTimeString = configuration["RATING_CLOSING_TIME"];

        if (string.IsNullOrEmpty(ratingClosingTimeString))
        {
            throw new InvalidOperationException("Environment variable, RATING_CLOSING_TIME, has not been configured.");
        }

        string[] validFormats = {
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:sszzz"
        };
        try
        {
            return DateTimeOffset.ParseExact(
                ratingClosingTimeString,
                validFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
            );
        }
        catch (FormatException)
        {
            throw new InvalidOperationException(
                $"Environment variable, RATING_CLOSING_TIME, has an invalid format. Expected formats: {string.Join(" or ", validFormats)}. Received: '{ratingClosingTimeString}'."
            );
        }
    }
}