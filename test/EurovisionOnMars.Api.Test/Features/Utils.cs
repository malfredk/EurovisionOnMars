using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Test.Features;

public class Utils
{
    public static int COUNTRY_ID = 8;
    public static int COUNTRY_NUMBER = 5;
    public static string COUNTRY_NAME = "australia";
    public static int COUNTRY_RANK = 7;

    public static string PLAYER_USERNAME = "lars";
    public static int PLAYER_ID = 1234;

    public static int RATING_ID = 77;
    
    // country

    public static Country CreateCountry()
    {
        var country = CreateInitialCountry();
        country.SetActualRank(COUNTRY_RANK);
        return country;
    }

    public static Country CreateInitialCountry() 
    {
        return CreateInitialCountry(COUNTRY_NUMBER);
    }


    public static Country CreateInitialCountry(int number)
    {
        return new Country(number, COUNTRY_NAME)
        {
            Id = COUNTRY_ID
        };
    }
    
    // player

    public static Player CreateInitialPlayerWithOneCountry()
    {
        return CreateInitialPlayerWithOneCountry(PLAYER_ID);
    }

    public static Player CreateInitialPlayerWithOneCountry(int playerId)
    {
        var country = CreateInitialCountry();
        return CreateInitialPlayer(playerId, [country]);
    }

    public static Player CreateInitialPlayer(ImmutableList<Country> countries)
    {
        return CreateInitialPlayer(PLAYER_ID, countries);
    }

    private static Player CreateInitialPlayer(int playerId, ImmutableList<Country> countries)
    {
        return new Player(PLAYER_USERNAME, countries)
        {
            Id = playerId
        };
    }


    // player game result

    public static PlayerGameResult CreatePlayerGameResult(int? rank, int? totalPoints)
    {
        var playerGameResult = CreateInitialPlayerGameResult(PLAYER_ID);
        playerGameResult.Rank = rank;
        playerGameResult.TotalPoints = totalPoints;

        return playerGameResult;
    }

    public static PlayerGameResult CreateInitialPlayerGameResult()
    {
        return CreateInitialPlayerGameResult(PLAYER_ID);
    }

    public static PlayerGameResult CreateInitialPlayerGameResult(int playerId)
    {
        var player = CreateInitialPlayerWithOneCountry(playerId);
        return new PlayerGameResult(player);
    }

    // player rating

    public static PlayerRating CreatePlayerRating(int category1Points, int category2Points, int category3Points, int rank)
    {
        var rating = CreateInitialPlayerRating();
        rating.SetPoints(category1Points, category2Points, category3Points);

        rating.Prediction.SetCalculatedRank(rank);

        return rating;
    }

    public static PlayerRating CreateInitialPlayerRating()
    {
        var country = CreateInitialCountry();
        var player = CreateInitialPlayerWithOneCountry();

        var rating = new PlayerRating(player, country)
        {
            Id = RATING_ID,
        };
        return rating;
    }

    // rating game result

    public static RatingGameResult CreateRatingGameResult(int? difference, int? bonusPoints)
    {
        var player = Utils.CreateInitialPlayerWithOneCountry();
        var ratingGameResult = player.PlayerRatings.First().RatingGameResult;

        ratingGameResult.RankDifference = difference;
        ratingGameResult.BonusPoints = bonusPoints;

        return ratingGameResult;
    }
}
