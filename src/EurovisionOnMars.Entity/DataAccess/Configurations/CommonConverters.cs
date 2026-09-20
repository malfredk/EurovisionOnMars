using EurovisionOnMars.Entity.Countries;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Entity.DataAccess.Configurations;

internal static class CommonConverters
{
    internal static readonly ValueConverter<CountryPosition?, int?> NullableCountryPositionConverter = 
        new ValueConverter<CountryPosition?, int?>(
            countryPosition => countryPosition == null ? null : countryPosition.Value,
            value => value == null ? null : new CountryPosition(value.Value));
}
