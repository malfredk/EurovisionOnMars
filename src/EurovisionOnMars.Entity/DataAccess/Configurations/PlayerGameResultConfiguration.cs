using EurovisionOnMars.Entity.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Entity.DataAccess.Configurations;

public class PlayerGameResultConfiguration : IEntityTypeConfiguration<PlayerGameResult>
{
    public void Configure(EntityTypeBuilder<PlayerGameResult> playerGameResult)
    {
        var playerRankConverter = CreatePlayerRankConverter();
        playerGameResult
            .Property(playerGameResult => playerGameResult.Rank)
            .HasConversion(playerRankConverter);
    }

    private static ValueConverter<PlayerRank?, int?> CreatePlayerRankConverter()
    {
        return new ValueConverter<PlayerRank?, int?>(
            position => position == null ? null : position.Value,
            value => value == null ? null : new PlayerRank(value.Value));
    }
}