using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test.Players;

public class PlayerTest
{
    private static readonly Username Username = new Username("hiæøÅ1278");

    [Fact]
    public void Player_Valid() {         
        // arrange
        var countries = GetCountries();

        // act
        var player = new Player(Username, countries);

        // assert
        Assert.Equal(Username, player.Username);
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
    [MemberData(nameof(NoCountriesTestData))]
    public void Player_MissingCountries_ThrowException(ImmutableList<Country> countries)
    {
        // act and assert
        Assert.Throws<InvalidOperationException>(() => new Player(Username, countries));
    }

    public static IEnumerable<object[]> NoCountriesTestData =>
    new List<object[]>
    {
        new object[] { null! },                                       // null
        new object[] { ImmutableList<Country>.Empty }                 // empty
    };

    private ImmutableList<Country> GetCountries()
    {
        return new List<Country>
        {
            new Country(new CountryPosition(1), new CountryName("norge")),
            new Country(new CountryPosition(3), new CountryName("danmark"))
        }.ToImmutableList();
    }
}
