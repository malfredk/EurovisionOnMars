namespace EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;

public static class DependencyInjection
{
    public static IServiceCollection AddCalculateGameResultsFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICalculateGameResultsRepository,
            CalculateGameResultsRepository>();

        services.AddScoped<
            ICalculateGameResultsService,
            CalculateGameResultsService>();

        return services;
    }
}
