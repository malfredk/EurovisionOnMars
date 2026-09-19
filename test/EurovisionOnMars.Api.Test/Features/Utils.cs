using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Test.Features;

public class Utils
{
    public const int COUNTRY_ID = 8;
    public static readonly CountryPosition COUNTRY_NUMBER = new(5);
    public static readonly CountryName COUNTRY_NAME = new("australia");
    public static readonly CountryPosition COUNTRY_RANK = new(7);

    public static readonly Username PLAYER_USERNAME = new("lars");
    public const int PLAYER_ID = 1234;

    public const int RATING_ID = 77;
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

    public static PlayerRating CreateInitialPlayerRating(
        int ratingId = RATING_ID,
        int playerId = PLAYER_ID
    )
    {
        var player = CreateInitialPlayer(playerId);

        var rating = player.PlayerRatings.FirstOrDefault()!;
        rating.Id = ratingId;
        rating.PlayerId = playerId;
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

    // rating game result

    public static RatingGameResult CreateRatingGameResult(int? difference, int bonusPoints)
    {
        var ratingGameResult = CreateRatingGameResult(difference);

        ratingGameResult.RankDifference = difference;
        ratingGameResult.BonusPoints = new BonusPoints(bonusPoints);

        return ratingGameResult;
    }

    public static RatingGameResult CreateRatingGameResult(int? difference)
    {
        var ratingGameResult = CreateInitialRatingGameResult();

        ratingGameResult.RankDifference = difference;

        return ratingGameResult;
    }

    public static RatingGameResult CreateInitialRatingGameResult()
    {
        var player = CreateInitialPlayer();
        return player.PlayerRatings.First().RatingGameResult;
    }

    // player game result

    public static PlayerGameResult CreateInitialPlayerGameResult(int playerId = PLAYER_ID)
    {
        var player = CreateInitialPlayer(playerId);
        var playerGameResult = player.PlayerGameResult;
        playerGameResult.PlayerId = playerId;
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

    // update player rating request

    public static UpdatePlayerRatingRequestDto CreateUpdatePlayerRatingRequest()
    {
        return new UpdatePlayerRatingRequestDto()
        {
            Category1Points = CATEGORY1_POINTS.Value,
            Category2Points = CATEGORY2_POINTS.Value,
            Category3Points = CATEGORY3_POINTS.Value,
        };
    }
}
