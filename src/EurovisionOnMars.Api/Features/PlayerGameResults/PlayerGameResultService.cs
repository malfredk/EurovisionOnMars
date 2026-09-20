using EurovisionOnMars.Api.Features.RatingGameResults;
using EurovisionOnMars.Entity.Players;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

public interface IPlayerGameResultService
{
    Task<ImmutableList<PlayerGameResult>> GetPlayerGameResults();
}

public class PlayerGameResultService : IPlayerGameResultService
{
    private readonly IPlayerGameResultRepository _playerGameResultRepository;
    private readonly ILogger<PlayerGameResultService> _logger;
    private readonly IRatingGameResultService _ratingGameResultService;

    public PlayerGameResultService
        (
        IPlayerGameResultRepository playerGameResultRepository,
        ILogger<PlayerGameResultService> logger,
        IRatingGameResultService ratingGameResultService
        )
    {
        _playerGameResultRepository = playerGameResultRepository;
        _logger = logger;
        _ratingGameResultService = ratingGameResultService;
    }

    public async Task<ImmutableList<PlayerGameResult>> GetPlayerGameResults()
    {
        var playerGameResults = await _playerGameResultRepository.GetPlayerGameResults();
        return playerGameResults
            .OrderBy(p => p.Rank?.Value ?? int.MaxValue)
            .ToImmutableList();
    }
}