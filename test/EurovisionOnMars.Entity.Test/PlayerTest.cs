using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test;

public class PlayerTest
{
    [Fact]
    public void Player_Valid() {         
        // arrange
        var countries = GetCountries();
        var username = "hiæøÅ1278";

        // act
        var player = new Player(username, countries);

        // assert
        Assert.Equal(username, player.Username);
        Assert.Equal(countries.Count, player.PlayerRatings.Count);
        Assert.Equal(countries, player.PlayerRatings.Select(pr => pr.Country));
        Assert.All(player.PlayerRatings, pr =>
        {
            Assert.Equal(player, pr.Player);
            Assert.NotNull(pr.Country);
            Assert.NotNull(pr.Prediction);
            Assert.NotNull(pr.RatingGameResult);
        });
        Assert.Equal(player, player.PlayerGameResult.Player);
    }

    [Theory]
    [InlineData("")]
    [InlineData("hei ho")]
    [InlineData("j*n")]
    [InlineData("=ndwnfks")]
    [InlineData("tretten123456")]
    public void Player_InvalidUsername(string username)
    {
        // arrange
        var countries = GetCountries();

        // act & assert
        Assert.Throws<ArgumentException>(() => new Player(username, countries));
    }

    private ImmutableList<Country> GetCountries()
    {
        return new List<Country>
        {
            new Country(1, "norge"),
            new Country(3, "danmark")
        }.ToImmutableList();
    }
}
