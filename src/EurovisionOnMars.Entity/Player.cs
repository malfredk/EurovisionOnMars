namespace EurovisionOnMars.Entity;

public record Player : IdBase
{
    public required string Username { get; set; }
    public List<Rating>? Ratings { get; set; }
    public PlayerResult? PlayerResult { get; set; }
}