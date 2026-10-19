namespace EurovisionOnMars.Api.Features.Countries.SetRank;

public static class DependencyInjection
{
    public static IServiceCollection AddSetRankFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            ISetRankRepository,
            SetRankRepository>();

        services.AddScoped<
            ISetRankService,
            SetRankService>();

        return services;
    }
}
