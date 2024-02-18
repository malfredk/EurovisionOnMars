namespace EurovisionOnMars.Entity;

public record Rating : IdBase
{
    public int? Category1Points { get; set; }
    public int? Category2Points { get; set; }
    public int? Category3Points { get; set; }
    public required int PlayerId { get; init; }
    public Player? Player { get; set; }
    public int? PointsSum { get; set; }
    public int? Ranking { get; set; }
}
