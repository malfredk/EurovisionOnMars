using EurovisionOnMars.Domain.Players;
using EurovisionOnMars.Domain.Players.Predictions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Domain.DataAccess.Configurations.Players.PlayerRatings;

public class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
{
    public void Configure(EntityTypeBuilder<Prediction> prediction)
    {
        var calculatedRankConverter = CommonConverters.NullableCountryPositionConverter;
        prediction
            .Property(prediction => prediction.CalculatedRank)
            .HasConversion(calculatedRankConverter);

        var tieBreakDemotionConverter = CreateTieBreakDemotionConverter();
        prediction
            .Property(prediction => prediction.TieBreakDemotion)
            .HasConversion(tieBreakDemotionConverter);
    }

    private static ValueConverter<TieBreakDemotion?, int?> CreateTieBreakDemotionConverter()
    {
        return new ValueConverter<TieBreakDemotion?, int?>(
            tieBreakDemotion => tieBreakDemotion == null ? null : tieBreakDemotion.Value,
            value => value == null ? null : new TieBreakDemotion(value.Value));
    }
}