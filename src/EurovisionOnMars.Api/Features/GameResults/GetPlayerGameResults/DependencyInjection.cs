namespace EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;

public static class DependencyInjection
{
    public static IServiceCollection AddGetPlayerGameResultsFeature(
        this IServiceCollection services)
    {
        services.AddTransient<
            IPlayerGameResultMapper,
            PlayerGameResultMapper>();

        services.AddScoped<
            IGetPlayerGameResultsRepository,
            GetPlayerGameResultsRepository>();

        services.AddScoped<
            IGetPlayerGameResultsService,
            GetPlayerGameResultsService>();

        return services;
    }
}
