using EurovisionOnMars.Api.Features.Countries.CreateCountry;
using EurovisionOnMars.Api.Features.Countries.GetCountries;
using EurovisionOnMars.Api.Features.Countries.SetRank;

namespace EurovisionOnMars.Api.Features.Countries;

public static class DependencyInjection
{
    public static IServiceCollection AddCountriesFeatures(
        this IServiceCollection services)
    {
        services.AddTransient<
            ICountryMapper,
            CountryMapper>();

        services
            .AddCreateCountryFeature()
            .AddGetCountriesFeature()
            .AddSetRankFeature();

        return services;
    }
}
