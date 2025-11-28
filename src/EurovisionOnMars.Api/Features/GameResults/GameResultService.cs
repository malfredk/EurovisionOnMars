using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Api.Features.RatingGameResults;

namespace EurovisionOnMars.Api.Features.GameResults;

public interface IGameResultService
{
    Task CalculateGameResults();
}

public class GameResultService : IGameResultService
{
    private readonly IPlayerGameResultService _playerGameResultService;
    private readonly IRatingGameResultService _ratingGameResultService;
    private readonly ILogger<GameResultService> _logger;

    public GameResultService
        (
        IPlayerGameResultService playerGameResultService,
        IRatingGameResultService ratingGameResultService,
        ILogger<GameResultService> logger
        )
    {
        _playerGameResultService = playerGameResultService;
        _ratingGameResultService = ratingGameResultService;
        _logger = logger;
    }

    public async Task CalculateGameResults()
    {
        await _ratingGameResultService.CalculateRatingGameResults();
        await _playerGameResultService.CalculatePlayerGameResults();
    }
}