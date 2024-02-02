namespace EurovisionOnMars.Entity;

public record Player (string Username)
{
    public int Id { get; init; }
}
