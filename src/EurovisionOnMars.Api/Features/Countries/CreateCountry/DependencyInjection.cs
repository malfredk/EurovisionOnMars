namespace EurovisionOnMars.Api.Features.Countries.CreateCountry;

public static class DependencyInjection
{
    public static IServiceCollection AddCreateCountryFeature(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICreateCountryRepository,
            CreateCountryRepository>();

        services.AddScoped<
            ICreateCountryService,
            CreateCountryService>();

        return services;
    }
}
