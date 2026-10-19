namespace EurovisionOnMars.Api.Features.Players.RateCountry;

public static class DependencyInjection
{
    public static IServiceCollection AddRateCountryFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            IRateCountryRepository,
            RateCountryRepository>();

        services.AddScoped<
            IRateCountryService,
            RateCountryService>();

        return services;
    }
}
