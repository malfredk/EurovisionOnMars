using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace EurovisionOnMars.Entity;

public record Player : IdBase
{
    public string Username { get; private set; } = null!;
    public List<PlayerRating> PlayerRatings { get; private set; }
    public PlayerGameResult PlayerGameResult { get; private set; }

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
        string pattern = @"^[a-zA-Z0-9æøåÆØÅ]*$";
        int maxLength = 12;

        var isValid = !string.IsNullOrEmpty(username)
            && Regex.IsMatch(username, pattern)
            && username.Length <= maxLength;

        if (!isValid)
        {
            throw new ArgumentException("Username can only contain letters and numbers");
        }
    }
}