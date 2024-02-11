using System.Diagnostics.CodeAnalysis;

namespace EurovisionOnMars.Entity;

public record Player : IdBase
{
    public required string Username { get; set; }
    public List<Rating>? Ratings { get; set; }
}
