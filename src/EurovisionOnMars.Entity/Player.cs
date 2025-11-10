using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace EurovisionOnMars.Entity;

public record Player : IdBase
{
    private static int USERNAME_MAX_LENGTH = 12;
    private static string USERNAME_PATTERN = @"^[a-zA-Z0-9æøåÆØÅ]*$";

    public string Username { get; init; } = null!;
    public List<PlayerRating> PlayerRatings { get; init; } = [];
    public PlayerGameResult PlayerGameResult { get; init; } = null!;

    private Player() { }

    public Player(string username, ImmutableList<Country> countries)
    {
        ValidateUsername(username);
        Username = username;

        PlayerRatings = countries
            .Select(c => new PlayerRating(this, c))
            .ToList();

        PlayerGameResult = new PlayerGameResult(this);
    }

    public void ValidateUsername(string username)
    {
        var isValid = !string.IsNullOrEmpty(username)
            && Regex.IsMatch(username, USERNAME_PATTERN)
            && username.Length <= USERNAME_MAX_LENGTH;

        if (!isValid)
        {
            throw new ArgumentException($"Username={username} contains invalid character or is too long.");
        }
    }
}