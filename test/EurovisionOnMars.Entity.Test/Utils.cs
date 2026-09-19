using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Test;

public class Utils
{
    public static readonly CountryPosition COUNTRY_NUMBER = new(5);
    public static readonly CountryName COUNTRY_NAME = new("australia");
    public static readonly CountryPosition COUNTRY_RANK = new(7);

    public static readonly Username PLAYER_USERNAME = new("lars");

    public static readonly Points CATEGORY1_POINTS = new(4);
    public static readonly Points CATEGORY2_POINTS = new(12);
    public static readonly Points CATEGORY3_POINTS = new(8);

    public static readonly CountryPosition PREDICTION_CALCULATED_RANK = new(20);
    public static readonly CountryPosition PREDICTION_RANK = new(21);
    public const int TIE_BREAK_DEMOTION = 1;

    public const int PLAYER_GAME_RESULT_RANK = 10;
    public const int PLAYER_GAME_RESULT_POINTS = 300;

    // country

    public static Country CreateInitialCountry()
    {
        return CreateInitialCountry(COUNTRY_NUMBER);
    }

    public static Country CreateInitialCountry(CountryPosition number)
    {
        return new Country(number, COUNTRY_NAME);
    }

    public static Country CreateRankedCountry()
    {
        var country = CreateInitialCountry();
        country.SetActualRank(COUNTRY_RANK);
        return country;
    }
    
    // player

    public static Player CreateInitialPlayer()
    {
        var country = CreateInitialCountry();
        return CreateInitialPlayer(country);
    }

    public static Player CreateInitialPlayer(Country country)
    {
        return CreateInitialPlayer([country]);
    }

    public static Player CreateInitialPlayer(ImmutableList<Country> countries)
    {
        return new Player(PLAYER_USERNAME, countries);
    }

    // player rating

    public static PlayerRating CreateInitialPlayerRating()
    {
        var player = CreateInitialPlayer();

        var rating = player.PlayerRatings.FirstOrDefault()!;
        return rating;
    }

    public static PlayerRating CreatePlayerRating()
    {
        return CreatePlayerRating(
            CATEGORY1_POINTS,
            CATEGORY2_POINTS,
            CATEGORY3_POINTS,
            PREDICTION_CALCULATED_RANK
        );
    }

    public static PlayerRating CreatePlayerRating(
        int category1Points,
        int category2Points,
        int category3Points
    )
    {
        return CreatePlayerRating(
            new Points(category1Points),
            new Points(category2Points),
            new Points(category3Points)
        );
    }

    public static PlayerRating CreatePlayerRating(
        Points category1Points,
        Points category2Points,
        Points category3Points
    )
    {
        return CreatePlayerRating(
            category1Points, 
            category2Points, 
            category3Points, 
            PREDICTION_CALCULATED_RANK
        );
    }

    public static PlayerRating CreatePlayerRating(
        Points category1Points, 
        Points category2Points, 
        Points category3Points, 
        CountryPosition rank
    )
    {
        var rating = CreateInitialPlayerRating();
        rating.SetPoints(
            category1Points,
            category2Points, 
            category3Points
            );

        rating.Prediction.SetCalculatedRank(rank);
        rating.Prediction.SetTieBreakDemotion(TIE_BREAK_DEMOTION);

        return rating;
    }

    // player game result

    public static PlayerGameResult CreateInitialPlayerGameResult()
    {
        var player = CreateInitialPlayer();
        var playerGameResult = player.PlayerGameResult;
        return playerGameResult;
    }

    public static PlayerGameResult CreatePlayerGameResult(
        int totalPoints = PLAYER_GAME_RESULT_RANK
    )
    {
        var playerGameResult = CreateInitialPlayerGameResult();
        playerGameResult.SetTotalPoints(totalPoints);

        return playerGameResult;
    }

    public static PlayerGameResult CreatePlayerGameResult(
        int rank = PLAYER_GAME_RESULT_RANK, 
        int totalPoints = PLAYER_GAME_RESULT_RANK
    )
    {
        var playerGameResult = CreatePlayerGameResult(totalPoints);
        playerGameResult.SetRank(rank);

        return playerGameResult;
    }
}
