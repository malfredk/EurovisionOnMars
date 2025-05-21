using EurovisionOnMars.Api.Configurations;
using Microsoft.Extensions.Configuration;

namespace EurovisionOnMars.Api.Test.Configurations;

public class ConfigurationValidatorTest
{
    private IConfiguration BuildConfig(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void GetAndValidateRatingClosingTime_ValidFormat(string input, DateTimeOffset expectedResult)
    {
        // Arrange
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["RATING_CLOSING_TIME"] = input
        });

        // Act
        var actualResult = ConfigurationValidator.GetAndValidateRatingClosingTime(config);

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { "2025-05-21T23:50:00Z", new DateTimeOffset(2025, 5, 21, 23, 50, 0, TimeSpan.Zero) };
        yield return new object[] { "2025-05-06T23:50:00+02:00", new DateTimeOffset(2025, 5, 6, 21, 50, 0, TimeSpan.Zero) };
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetAndValidateRatingClosingTime_Missing(string input)
    {
        // Arrange
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["RATING_CLOSING_TIME"] = input
        });

        // Act and assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            ConfigurationValidator.GetAndValidateRatingClosingTime(config));
        Assert.Equal("Environment variable, RATING_CLOSING_TIME, has not been configured.", ex.Message);
    }

    [Theory]
    [InlineData("swims")]
    [InlineData("2025-05-21T23:50:00")]
    [InlineData("2025-05-21T23:50:00UTC")]
    [InlineData("2025-05-2123:50:00Z")]
    [InlineData("2025-05-21T23:50:00-02")]
    public void GetAndValidateRatingClosingTime_InvalidFormat(string input)
    {
        // Arrange
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["RATING_CLOSING_TIME"] = input
        });

        // Act and assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            ConfigurationValidator.GetAndValidateRatingClosingTime(config));
        Assert.Contains("Environment variable, RATING_CLOSING_TIME, has an invalid format.", ex.Message);
    }
}