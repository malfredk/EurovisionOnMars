namespace EurovisionOnMars.Api.Features.Players.CreatePlayer;

public static class DependencyInjection
{
    public static IServiceCollection AddCreatePlayerFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICreatePlayerRepository,
            CreatePlayerRepository>();

        services.AddScoped<
            ICreatePlayerService,
            CreatePlayerService>();

        return services;
    }
}
