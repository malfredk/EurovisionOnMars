using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity.Players.PlayerRatings;

public class RatingGameResult : IdBase
{
    public int? RankDifference { get; private set; } // actual minus predicted
    public BonusPoints? BonusPoints { get; private set; }
    public int PlayerRatingId { get; private set; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; private set; }

    private RatingGameResult() { }

    internal RatingGameResult(PlayerRating playerRating)
    {
        PlayerRating = playerRating;
    }

    internal void SetRankDifference(int rankDifference)
    {
        RankDifference = rankDifference;
    }

    internal void SetBonusPoints(BonusPoints bonusPoints)
    {
        BonusPoints = bonusPoints;
    }
}