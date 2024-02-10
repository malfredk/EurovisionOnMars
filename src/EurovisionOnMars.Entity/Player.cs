using System.Diagnostics.CodeAnalysis;

namespace EurovisionOnMars.Entity;

public record Player
{
    public int Id { get; init; }
    public required string Username { get; set; }
    public List<Rating>? Ratings { get; set; }

    [SetsRequiredMembersAttribute]
    public Player(string username)
    {
        Username = username;
    }
}
