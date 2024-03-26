namespace EurovisionOnMars.Entity;

public record PlayerResult : IdBase
{
    public int? Ranking { get; set; }
    public int? Score { get; set; }
    public int PlayerId {  get; init; }
    public Player? Player { get; set; }
}
