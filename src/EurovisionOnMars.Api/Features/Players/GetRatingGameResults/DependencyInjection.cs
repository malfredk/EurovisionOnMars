namespace EurovisionOnMars.Api.Features.Players.GetRatingGameResults;

public static class DependencyInjection
{
    public static IServiceCollection AddGetRatingGameResultsFeature(
        this IServiceCollection services)
    {
        services.AddTransient<
            IRatingGameResultMapper,
            RatingGameResultMapper>();

        services.AddScoped<
            IGetRatingGameResultsRepository,
            GetRatingGameResultsRepository>();

        services.AddScoped<
            IGetRatingGameResultsService,
            GetRatingGameResultsService>();

        return services;
    }
}
