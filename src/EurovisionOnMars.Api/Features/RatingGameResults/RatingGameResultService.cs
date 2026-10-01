using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

public interface IRatingGameResultService
{
    Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId);
}

public class RatingGameResultService : IRatingGameResultService
{
    private readonly IRatingGameResultRepository _ratingGameResultRepository;
    private readonly IGetRatingsService _playerRatingService;
    private readonly ILogger<RatingGameResultService> _logger;

    public RatingGameResultService
        (
        IRatingGameResultRepository ratingResultRepository,
        IGetRatingsService playerRatingService,
        ILogger<RatingGameResultService> logger
        )
    {
        _ratingGameResultRepository = ratingResultRepository;
        _playerRatingService = playerRatingService;
        _logger = logger;
    }

    public async Task<ImmutableList<RatingGameResult>> GetRatingGameResults(int playerId)
    {
        return await _ratingGameResultRepository.GetRatingGameResults(playerId);
    }
}