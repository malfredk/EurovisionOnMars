namespace EurovisionOnMars.Entity;

public record Country : IdBase
{
    public required int Number { get; set; }
    public required string Name { get; set; }
    public int? Ranking { get; set; }
    public List<Rating>? Ratings { get; set; }
}