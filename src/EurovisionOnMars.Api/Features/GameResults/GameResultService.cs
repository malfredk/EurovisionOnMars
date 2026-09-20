namespace EurovisionOnMars.Api.Features.GameResults;

public interface IGameResultService
{
    Task CalculateGameResults();
}

public class GameResultService : IGameResultService
{
    private readonly IPlayerRanksCalculator _playerRanksCalculator;
    private readonly IGameResultRepository _gameResultRepository;
    private readonly ILogger<GameResultService> _logger;

    public GameResultService
        (
        IPlayerRanksCalculator playerRanksCalculator,
        IGameResultRepository gameResultRepository,
        ILogger<GameResultService> logger
        )
    {
        _playerRanksCalculator = playerRanksCalculator;
        _gameResultRepository = gameResultRepository;
        _logger = logger;
    }

    public async Task CalculateGameResults()
    {
        var players = await _gameResultRepository.GetPlayers();

        foreach (var player in players)
        {
            _logger.LogDebug("Calculating game points for player with id={playerId}.", player.Id);
            player.CalculateGamePoints();
        }

        _playerRanksCalculator.CalculatePlayerRanks(players);

        await _gameResultRepository.SaveChanges();
    }
}