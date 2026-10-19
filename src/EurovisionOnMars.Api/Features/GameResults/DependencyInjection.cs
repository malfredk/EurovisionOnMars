using EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;
using EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;

namespace EurovisionOnMars.Api.Features.GameResults;

public static class DependencyInjection
{
    public static IServiceCollection AddGameResultsFeatures(
        this IServiceCollection services)
    {
        services
            .AddCalculateGameResultsFeature()
            .AddGetPlayerGameResultsFeature();

        return services;
    }
}
