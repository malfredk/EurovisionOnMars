namespace EurovisionOnMars.Entity;

public record RatingResult : IdBase
{
    public int? RankingDifference { get; set; } // actual minus expected
    public int? BonusPoints { get; set; }
    public int RatingId { get; init; }
    public Rating Rating { get; set; } = null!;
}