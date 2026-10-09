using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players.GameResults;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Domain.Players.Predictions;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Immutable;

namespace EurovisionOnMars.Domain.Players;

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

    public void CalculateGamePoints()
    {
        RatingGameResultsCalculator.Calculate(PlayerRatings);
        PlayerTotalPointsCalculator.Calculate(PlayerRatings, PlayerGameResult);
    }

    public void RateCountry(
        int ratingId,
        Points category1Points,
        Points category2Points,
        Points category3Points
        )
    {
        var rating = GetRating(ratingId);

        SpecialPointsPolicy.Validate(
            rating,
            PlayerRatings,
            category1Points,
            category2Points,
            category3Points);

        var oldTotalPoints = rating.Prediction.TotalGivenPoints;

        rating.SetPoints(
            category1Points,
            category2Points,
            category3Points);

        PredictionsCalculator.Calculate(rating, PlayerRatings, oldTotalPoints);
    }

    private PlayerRating GetRating(int ratingId)
    {
        return PlayerRatings
            .SingleOrDefault(r => r.Id == ratingId)
            ?? throw new KeyNotFoundException(
                $"No player rating with id={ratingId} exists for this player.");
    }

    public void ResolveTieBreak(List<int> orderedPredictionIds)
    {
        var predictions = GetPredictions();
        TieBreakResolver.Resolve(predictions, orderedPredictionIds);
    }

    private List<Prediction> GetPredictions()
    {
        return PlayerRatings
            .Select(r => r.Prediction)
            .ToList();
    }
}