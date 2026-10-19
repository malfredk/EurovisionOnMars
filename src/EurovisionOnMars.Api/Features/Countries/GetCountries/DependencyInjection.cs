namespace EurovisionOnMars.Api.Features.Countries.GetCountries;

public static class DependencyInjection
{
    public static IServiceCollection AddGetCountriesFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            IGetCountriesRepository,
            GetCountriesRepository>();

        services.AddScoped<
            IGetCountriesService,
            GetCountriesService>();

        return services;
    }
}
