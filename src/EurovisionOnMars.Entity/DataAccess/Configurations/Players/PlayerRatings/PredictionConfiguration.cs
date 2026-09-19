using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EurovisionOnMars.Entity.DataAccess.Configurations.Players.PlayerRatings;

public class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
{
    public void Configure(EntityTypeBuilder<Prediction> prediction)
    {
        var calculatedRankConverter = CommonConverters.NullableCountryPositionConverter;
        prediction
            .Property(prediction => prediction.CalculatedRank)
            .HasConversion(calculatedRankConverter);
    }
}