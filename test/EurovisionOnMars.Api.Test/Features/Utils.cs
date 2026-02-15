using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Test.Features;
public static class TestEntityExtensions
{
    public static T WithId<T>(this T entity, int id) where T : IdBase
        => (T)(entity with { Id = id });
}

public class Utils
{
    public const int COUNTRY_ID = 8;
    public const int COUNTRY_NUMBER = 5;
    public const string COUNTRY_NAME = "australia";
    public const int COUNTRY_RANK = 7;

    public const string PLAYER_USERNAME = "lars";
    public const int PLAYER_ID = 1234;

    public const int RATING_ID = 77;
    public const int CATEGORY1_POINTS = 4;
    public const int CATEGORY2_POINTS = 12;
    public const int CATEGORY3_POINTS = 8;

    public const int PREDICTION_ID = 8888;
    public const int PREDICTION_TOTAL_POINTS = 24;
    public const int PREDICTION_CALCULATED_RANK = 20;
    public const int PREDICTION_RANK = 21;
    public const int TIE_BREAK_DEMOTION = 1;
    

    public const int PLAYER_GAME_RESULT_RANK = 10;
    public const int PLAYER_GAME_RESULT_POINTS = 300;

    // country

    public static Country CreateInitialCountry(int number = COUNTRY_NUMBER)
    {
        return new Country(number, COUNTRY_NAME)
        {
            Id = COUNTRY_ID
        };
    }

    public static Country CreateRankedCountry()
    {
        var country = CreateInitialCountry();
        country.SetActualRank(COUNTRY_RANK);
        return country;
    }
    
    // player

    public static Player CreateInitialPlayer(int playerId = PLAYER_ID)
    {
        var country = CreateInitialCountry();
        return CreateInitialPlayer(country, playerId);
    }

    public static Player CreateInitialPlayer(Country country, int playerId = PLAYER_ID)
    {
        return new Player(PLAYER_USERNAME, [country])
        {
            Id = playerId
        };
    }

    // player rating

    public static PlayerRating CreateInitialPlayerRating(int ratingId = RATING_ID)
    {
        var player = CreateInitialPlayer();

        return player.PlayerRatings.FirstOrDefault()!.WithId(ratingId);
    }

    public static PlayerRating CreatePlayerRating(
        int category1Points = CATEGORY1_POINTS, 
        int category2Points = CATEGORY2_POINTS, 
        int category3Points = CATEGORY3_POINTS, 
        int rank = PREDICTION_CALCULATED_RANK
    )
    {
        var rating = CreateInitialPlayerRating();
        rating.SetPoints(category1Points, category2Points, category3Points);

        rating.Prediction.WithId(PREDICTION_ID);
        rating.Prediction.SetCalculatedRank(rank);
        rating.Prediction.SetTieBreakDemotion(TIE_BREAK_DEMOTION);

        return rating;
    }

    // rating game result

    public static RatingGameResult CreateRatingGameResult(int? difference, int? bonusPoints)
    {
        var player = CreateInitialPlayer();
        var ratingGameResult = player.PlayerRatings.First().RatingGameResult;

        ratingGameResult.RankDifference = difference;
        ratingGameResult.BonusPoints = bonusPoints;

        return ratingGameResult;
    }

    // player game result

    public static PlayerGameResult CreateInitialPlayerGameResult(int playerId = PLAYER_ID)
    {
        var player = CreateInitialPlayer(playerId);
        return player.PlayerGameResult;
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

    // update player rating request

    public static UpdatePlayerRatingRequestDto CreateUpdatePlayerRatingRequest()
    {
        return new UpdatePlayerRatingRequestDto()
        {
            Category1Points = CATEGORY1_POINTS,
            Category2Points = CATEGORY2_POINTS,
            Category3Points = CATEGORY3_POINTS,
        };
    }
}
