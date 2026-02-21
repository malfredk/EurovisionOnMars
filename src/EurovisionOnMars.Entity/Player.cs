using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace EurovisionOnMars.Entity;

public class Player : IdBase
{
    private static int USERNAME_MAX_LENGTH = 12;
    private static string USERNAME_PATTERN = @"^[a-zA-Z0-9æøåÆØÅ]*$";

    public string Username { get; private set; } = null!;
    public List<PlayerRating> PlayerRatings { get; private set; } = [];
    public PlayerGameResult PlayerGameResult { get; private set; } = null!;

    private Player() { }

    public Player(string username, ImmutableList<Country> countries)
    {
        ValidateUsername(username);
        ValidateCountries(countries);
        Username = username;

        PlayerRatings = countries
            .Select(c => new PlayerRating(this, c))
            .ToList();

        PlayerGameResult = new PlayerGameResult(this);
    }

    public static void ValidateUsername(string username)
    {
        var isValid = !string.IsNullOrEmpty(username)
            && Regex.IsMatch(username, USERNAME_PATTERN)
            && username.Length <= USERNAME_MAX_LENGTH;

        if (!isValid)
        {
            throw new ArgumentException($"Username={username} contains invalid character or is too long.");
        }
    }

    private void ValidateCountries(ImmutableList<Country> countries)
    {
        if (countries.IsNullOrEmpty())
        {
            throw new InvalidOperationException("Country list is empty.");
        }
    }
}