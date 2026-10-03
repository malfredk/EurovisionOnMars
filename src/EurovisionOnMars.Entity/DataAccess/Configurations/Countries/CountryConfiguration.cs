using EurovisionOnMars.Entity.Countries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Entity.DataAccess.Configurations.Countries;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> country)
    {
        country
            .HasIndex(country => country.Number)
            .IsUnique();

        var countryNameConverter = CreateCountryNameConverter();
        country
            .Property(country => country.Name)
            .HasConversion(countryNameConverter);


        var countryNumberConverter = CreateCountryPositionConverter();
        country
            .Property(country => country.Number)
            .HasConversion(countryNumberConverter);

        var countryRankConverter = CommonConverters.NullableCountryPositionConverter;
        country
            .Property(country => country.ActualRank)
            .HasConversion(countryRankConverter);
    }

    private static ValueConverter<CountryName, string> CreateCountryNameConverter()
    {
        return new ValueConverter<CountryName, string>(
            name => name.Value,
            value => new CountryName(value));
    }

    private static ValueConverter<CountryPosition, int> CreateCountryPositionConverter()
    {
        return new ValueConverter<CountryPosition, int>(
            position => position.Value,
            value => new CountryPosition(value));
    }
}