namespace EurovisionOnMars.Entity;

public record Rating
{
    public int Id { get; init; }
    public int? Category1 { get; set; }
    public int? Category2 { get; set; }
    public int? Category3 { get; set; }
    public int PlayerId { get; set; }
    public required Player Player { get; set; }
}
