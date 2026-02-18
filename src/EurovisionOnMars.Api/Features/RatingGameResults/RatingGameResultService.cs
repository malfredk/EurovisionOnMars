using EurovisionOnMars.Api.Features.PlayerRatings;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultService
{
    Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId);
    Task CalculateRatingGameResults();
}

public class RatingGameResultService : IRatingGameResultService
{
    private readonly IRatingGameResultRepository _ratingGameResultRepository;
    private readonly IPlayerRatingService _playerRatingService;
    private readonly IRatingGameResultCalculator _ratingGameResultCalculator;
    private readonly ILogger<RatingGameResultService> _logger;

    public RatingGameResultService
        (
        IRatingGameResultRepository ratingResultRepository,
        IPlayerRatingService playerRatingService,
        IRatingGameResultCalculator ratingGameResultCalculator,
        ILogger<RatingGameResultService> logger
        )
    {
        _ratingGameResultRepository = ratingResultRepository;
        _playerRatingService = playerRatingService;
        _ratingGameResultCalculator = ratingGameResultCalculator;
        _logger = logger;
    }

    public async Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId)
    {
        return await _ratingGameResultRepository.GetRatingGameResults(playerId);
    }

    public async Task CalculateRatingGameResults()
    {
        var playerRatings = await _playerRatingService.GetAllPlayerRatings();
        CalculateRatingGameResults(playerRatings);
        await _ratingGameResultRepository.SaveChanges();
    }

    private void CalculateRatingGameResults(IReadOnlyList<PlayerRating> ratings)
    {
        var ratingsByPlayer = ratings
            .GroupBy(r => r.PlayerId).ToDictionary(g => g.Key, g => g.ToList());
        foreach (var rating in ratings)
        {
            _ratingGameResultCalculator
                .CalculateRatingGameResult(rating, ratingsByPlayer[rating.PlayerId]);
        }
    }
}