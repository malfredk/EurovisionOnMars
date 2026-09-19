using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Entity.DataAccess.Configurations.Players.PlayerRatings;

public class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
{
    public void Configure(EntityTypeBuilder<Prediction> prediction)
    {
        var calculatedRankConverter = CreateNullableCountryPositionConverter();
        prediction
            .Property(prediction => prediction.CalculatedRank)
            .HasConversion(calculatedRankConverter);
    }

    private static ValueConverter<CountryPosition?, int?> CreateNullableCountryPositionConverter()
    {
        return new ValueConverter<CountryPosition?, int?>(
            position => position == null ? null : position.Value,
            value => value == null ? null : new CountryPosition(value.Value));
    }
}