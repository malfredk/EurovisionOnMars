using EurovisionOnMars.Api.Features.Players.CreatePlayer;
using EurovisionOnMars.Api.Features.Players.GetPlayer;
using EurovisionOnMars.Api.Features.Players.GetRatingGameResults;
using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.Api.Features.Players.RateCountry;
using EurovisionOnMars.Api.Features.Players.ResolveTieBreak;

namespace EurovisionOnMars.Api.Features.Players;

public static class DependencyInjection
{
    public static IServiceCollection AddPlayersFeatures(
        this IServiceCollection services)
    {
        services
            .AddRatingTimeValidator()
            .AddCreatePlayerFeature()
            .AddGetPlayerFeature()
            .AddGetRatingGameResultsFeature()
            .AddGetRatingsFeature()
            .AddRateCountryFeature()
            .AddResolveTieBreakFeature();

        return services;
    }

    static IServiceCollection AddRatingTimeValidator(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeNow, DateTimeNow>();
        services.AddScoped<IRatingTimeValidator, RatingTimeValidator>();
        return services;
    }
}
