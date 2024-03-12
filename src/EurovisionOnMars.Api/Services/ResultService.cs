using EurovisionOnMars.Api.Repositories;

namespace EurovisionOnMars.Api.Services;

public interface IResultService
{
    Task CalculateResults();
}

public class ResultService : IResultService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRatingResultService _ratingResultService;
    private readonly IPlayerResultService _playerResultService;
    private readonly ILogger<ResultService> _logger;

    public ResultService
        (
        IPlayerRepository playerRepository,
        IRatingResultService ratingResultService, 
        IPlayerResultService playerResultService, 
        ILogger<ResultService> logger
        )
    {
        _playerRepository = playerRepository;
        _ratingResultService = ratingResultService;
        _playerResultService = playerResultService;
        _logger = logger;
    }

    public async Task CalculateResults()
    {
        // get all player ids
        var players = await _playerRepository.GetPlayers();
        var playerIds = players.Select(player => player.Id).ToList();

        // for each player, first calculate all rating results and then calculate total score
        foreach (var playerId in playerIds)
        {
            await _ratingResultService.CalculateRatingResults(playerId);
            await _playerResultService.CalculatePlayerScore(playerId);
        }

        // calculate ranking among players
        await _playerResultService.CalculatePlayerRankings();
    }
}