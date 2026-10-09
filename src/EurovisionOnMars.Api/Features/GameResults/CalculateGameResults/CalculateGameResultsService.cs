using EurovisionOnMars.Domain.Game;

namespace EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;

public interface ICalculateGameResultsService
{
    Task CalculateGameResults();
}

public class CalculateGameResultsService : ICalculateGameResultsService
{
    private readonly ILogger<CalculateGameResultsService> _logger;
    private readonly ICalculateGameResultsRepository _repository;
    private readonly PlayerRanksCalculator _playerRanksCalculator;

    public CalculateGameResultsService
        (
        ILogger<CalculateGameResultsService> logger,
        ICalculateGameResultsRepository repository,
        PlayerRanksCalculator playerRanksCalculator
        )
    {
        _logger = logger;
        _repository = repository;
        _playerRanksCalculator = playerRanksCalculator;
    }

    public async Task CalculateGameResults()
    {
        var players = await _repository.GetPlayers();

        foreach (var player in players)
        {
            _logger.LogDebug("Calculating game points for player with id={playerId}.", player.Id);
            player.CalculateGamePoints();
        }

        _playerRanksCalculator.CalculatePlayerRanks(players);

        await _repository.SaveChanges();
    }
}