namespace EurovisionOnMars.Entity;

public record Player : IdBase
{
    public required string Username { get; set; }
    public List<PlayerRating>? PlayerRatings { get; set; }
    public PlayerGameResult? PlayerGameResult { get; set; }
}