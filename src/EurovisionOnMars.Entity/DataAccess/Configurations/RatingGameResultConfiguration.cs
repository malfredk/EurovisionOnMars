using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Entity.DataAccess.Configurations;

public class RatingGameResultConfiguration : IEntityTypeConfiguration<RatingGameResult>
{
    public void Configure(EntityTypeBuilder<RatingGameResult> ratingGameResult)
    {
        var bonusPointsConverter = CreateBonusPointsConverter();
        ratingGameResult
            .Property(ratingGameResult => ratingGameResult.BonusPoints)
            .HasConversion(bonusPointsConverter);
    }

    private static ValueConverter<BonusPoints?, int?> CreateBonusPointsConverter()
    {
        return new ValueConverter<BonusPoints?, int?>(
            position => position == null ? null : position.Value,
            value => value == null ? null : new BonusPoints(value.Value));
    }
}