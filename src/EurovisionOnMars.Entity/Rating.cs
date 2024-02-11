namespace EurovisionOnMars.Entity;

public record Rating : IdBase
{
    public int? Category1 { get; set; }
    public int? Category2 { get; set; }
    public int? Category3 { get; set; }
    public required int PlayerId { get; init; }
    public required Player Player { get; set; }
}
