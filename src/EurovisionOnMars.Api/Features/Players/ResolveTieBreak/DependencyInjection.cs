namespace EurovisionOnMars.Api.Features.Players.ResolveTieBreak;

public static class DependencyInjection
{
    public static IServiceCollection AddResolveTieBreakFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            IResolveTieBreakRepository,
            ResolveTieBreakRepository>();

        services.AddScoped<
            IResolveTieBreakService,
            ResolveTieBreakService>();

        return services;
    }
}
