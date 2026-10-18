using EurovisionOnMars.Domain.Countries;
using EurovisionOnMars.Domain.Players.GameResults;
using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Domain.Players.Predictions;

namespace EurovisionOnMars.Domain.Players;

public class Player : IdBase
{
    private readonly List<PlayerRating> _playerRatings = [];
    public Username Username { get; private set; } = null!;
    public IReadOnlyList<PlayerRating> PlayerRatings => _playerRatings.AsReadOnly();
    public PlayerGameResult PlayerGameResult { get; private set; } = null!;

    private Player() { }

    public Player(Username username, IReadOnlyCollection<Country> countries)
    {
        ValidateCountries(countries);
        Username = username;

        _playerRatings.AddRange(countries
            .Select(c => new PlayerRating(this, c)));

        PlayerGameResult = new PlayerGameResult(this);
    }

    private void ValidateCountries(IReadOnlyCollection<Country> countries)
    {
        if (countries is null || countries.Count == 0)
        {
            throw new InvalidOperationException("Country list is empty; therefore user cannot be created.");
        }
    }

    public void CalculateGamePoints()
    {
        RatingGameResultsCalculator.Calculate(_playerRatings);
        PlayerTotalPointsCalculator.Calculate(_playerRatings, PlayerGameResult);
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
            _playerRatings,
            category1Points,
            category2Points,
            category3Points);

        var oldTotalPoints = rating.Prediction.TotalGivenPoints;

        rating.SetPoints(
            category1Points,
            category2Points,
            category3Points);

        PredictionsCalculator.Calculate(rating, _playerRatings, oldTotalPoints);
    }

    private PlayerRating GetRating(int ratingId)
    {
        return PlayerRatings
            .SingleOrDefault(r => r.Id == ratingId)
            ?? throw new KeyNotFoundException(
                $"No player rating with id={ratingId} exists for this player.");
    }

    public void ResolveTieBreak(IReadOnlyList<int> orderedPredictionIds)
    {
        var predictions = GetPredictions();
        TieBreakResolver.Resolve(predictions, orderedPredictionIds);
    }

    private IReadOnlyCollection<Prediction> GetPredictions()
    {
        return PlayerRatings
            .Select(r => r.Prediction)
            .ToList();
    }
}