using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features;

public class Utils
{
    public static string PLAYER_USERNAME = "lars";
    public static int PLAYER_ID = 1234;

    public static int COUNTRY_ID = 8;
    public static int COUNTRY_NUMBER = 5;
    public static string COUNTRY_NAME = "australia";
    public static int COUNTRY_RANK = 7;

    public static Player CreateInitialPlayerWithOneCountry()
    {
        var country = CreateInitialCountry();
        return CreateInitialPlayer([country]);
    }

    public static Player CreateInitialPlayer(ImmutableList<Country> countries)
    {
        return new Player(PLAYER_USERNAME, countries)
        {
            Id = PLAYER_ID
        };
    }

    public static Country CreateInitialCountry() 
    {
        return new Country(COUNTRY_NUMBER, COUNTRY_NAME)
        {
            Id = COUNTRY_ID
        };
    }
}
