namespace EurovisionOnMars.Api.Features.Players.GetRatings;

public static class DependencyInjection
{
    public static IServiceCollection AddGetRatingsFeature(
        this IServiceCollection services)
    {
        services.AddTransient<
            IPlayerRatingMapper,
            PlayerRatingMapper>();

        services.AddScoped<
            IGetRatingsRepository,
            GetRatingsRepository>();

        services.AddScoped<
            IGetRatingsService,
            GetRatingsService>();

        return services;
    }
}
