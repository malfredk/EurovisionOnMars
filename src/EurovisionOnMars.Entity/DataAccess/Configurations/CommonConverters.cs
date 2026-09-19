using EurovisionOnMars.Entity.Countries;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Entity.DataAccess.Configurations;

internal static class CommonConverters
{
    internal static readonly ValueConverter<CountryPosition?, int?> NullableCountryPositionConverter = 
        new ValueConverter<CountryPosition?, int?>(
            position => position == null ? null : position.Value,
            value => value == null ? null : new CountryPosition(value.Value));
}
