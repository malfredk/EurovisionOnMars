using System.Text.Json.Serialization;

namespace EurovisionOnMars.Entity;

public record RatingGameResult : IdBase
{
    public int? RankDifference { get; private set; } // actual minus predicted
    public int? BonusPoints { get; private set; }
    public int PlayerRatingId { get; private set; }
    [JsonIgnore]
    public PlayerRating? PlayerRating { get; private set; }

    private RatingGameResult() { }

    public RatingGameResult(PlayerRating playerRating)
    {
        PlayerRating = playerRating;
        PlayerRatingId = playerRating.Id;
    }
}