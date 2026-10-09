using EurovisionOnMars.Domain.Players.PlayerRatings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Domain.DataAccess.Configurations.Players.PlayerRatings;

public class PlayerRatingConfiguration : IEntityTypeConfiguration<PlayerRating>
{
    public void Configure(EntityTypeBuilder<PlayerRating> playerRating)
    {
        var pointsConverter = CreatePointsConverter();

        playerRating
            .Property(rating => rating.Category1Points)
            .HasConversion(pointsConverter);

        playerRating
            .Property(rating => rating.Category2Points)
            .HasConversion(pointsConverter);

        playerRating
            .Property(rating => rating.Category3Points)
            .HasConversion(pointsConverter);
    }

    private static ValueConverter<Points?, int?> CreatePointsConverter()
    {
        return new ValueConverter<Points?, int?>(
            points => points == null ? null : points.Value,
            value => value == null ? null : new Points(value.Value));
    }
}