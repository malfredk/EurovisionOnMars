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

    public CalculateGameResultsService
        (
        ILogger<CalculateGameResultsService> logger,
        ICalculateGameResultsRepository repository
        )
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task CalculateGameResults()
    {
        var players = await _repository.GetPlayers();

        foreach (var player in players)
        {
            _logger.LogDebug("Calculating game points for player with id={playerId}.", player.Id);
            player.CalculateGamePoints();
        }

        PlayerRanksCalculator.Calculate(players);

        await _repository.SaveChanges();
    }
}