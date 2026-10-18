using EurovisionOnMars.Domain.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EurovisionOnMars.Api.DataAccess.Configurations.Players;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> player)
    {
        player
            .HasIndex(p => p.Username)
            .IsUnique();

        var usernameConverter = CreateUsernameConverter();
        player
            .Property(player => player.Username)
            .HasConversion(usernameConverter)
            .IsRequired();

        player
            .Navigation(p => p.PlayerRatings)
            .HasField("_playerRatings")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static ValueConverter<Username, string> CreateUsernameConverter()
    {
        return new ValueConverter<Username, string>(
            username => username.Value,
            value => new Username(value));
    }
}