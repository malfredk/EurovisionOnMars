using EurovisionOnMars.Api.Test.TestData.Countries;
using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Api.Test.TestData.Players;

public static class PlayerTestFactory
{
    public static Player CreateInitialPlayer(int playerId = PlayerTestData.Id)
    {
        var country = CountryFactory.CreateInitialCountry();
        return CreateInitialPlayer(country, playerId);
    }

    public static Player CreateInitialPlayer(Country country, int playerId = PlayerTestData.Id)
    {
        var username = new Username(PlayerTestData.Username);
        return new Player(username, [country])
        {
            Id = playerId
        };
    }
}
