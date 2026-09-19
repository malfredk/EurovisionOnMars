using EurovisionOnMars.Entity.Countries;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;

namespace EurovisionOnMars.Entity.Players;

public class Player : IdBase
{
    public Username Username { get; private set; } = null!;
    public List<PlayerRating> PlayerRatings { get; private set; } = [];
    public PlayerGameResult PlayerGameResult { get; private set; } = null!;

    private Player() { }

    public Player(Username username, ImmutableList<Country> countries)
    {
        ValidateCountries(countries);
        Username = username;

        PlayerRatings = countries
            .Select(c => new PlayerRating(this, c))
            .ToList();

        PlayerGameResult = new PlayerGameResult(this);
    }

    private void ValidateCountries(ImmutableList<Country> countries)
    {
        if (countries.IsNullOrEmpty())
        {
            throw new InvalidOperationException("Country list is empty; therefore user cannot be created.");
        }
    }

    public void CalculateTotalPoints() // TODO: test
    {
        var totalPoints = PlayerRatings.Sum(rating =>
        {
            var result = rating.RatingGameResult
                ?? throw new InvalidOperationException(
                    "Rating game result is missing.");

            var bonusPoints = result.BonusPoints
                ?? throw new InvalidOperationException(
                    "Bonus points are missing.");

            var rankDifference = result.RankDifference
                ?? throw new InvalidOperationException(
                    "Rank difference is missing.");

            return bonusPoints + Math.Abs(rankDifference);
        });

        PlayerGameResult.SetTotalPoints(totalPoints);
    }
}