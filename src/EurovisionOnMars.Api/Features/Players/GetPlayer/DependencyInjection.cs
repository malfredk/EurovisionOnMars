namespace EurovisionOnMars.Api.Features.Players.GetPlayer;

public static class DependencyInjection
{
    public static IServiceCollection AddGetPlayerFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            IGetPlayerRepository,
            GetPlayerRepository>();

        services.AddScoped<
            IGetPlayerService,
            GetPlayerService>();

        return services;
    }
}
