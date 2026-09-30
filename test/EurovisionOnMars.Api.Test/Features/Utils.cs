using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Test.Features;

public class Utils
{
    public const int RATING_ID = 77;

    public static readonly CountryPosition PREDICTION_CALCULATED_RANK = new(10);
    public static readonly CountryPosition PREDICTION_RANK = new(11);
    public static readonly TieBreakDemotion TIE_BREAK_DEMOTION = new(1);

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
        int totalPoints
    )
    {
        var playerGameResult = CreateInitialPlayerGameResult();
        playerGameResult.SetTotalPoints(totalPoints);

        return playerGameResult;
    }

    public static PlayerGameResult CreatePlayerGameResult(
        PlayerRank rank, 
        int totalPoints = PLAYER_GAME_RESULT_POINTS
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
