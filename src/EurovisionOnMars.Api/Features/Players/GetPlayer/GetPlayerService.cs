using EurovisionOnMars.Domain.Players;

namespace EurovisionOnMars.Api.Features.Players.GetPlayer;

public interface IGetPlayerService
{
    Task<int> GetPlayer(string username);
}

public class GetPlayerService : IGetPlayerService
{
    private readonly ILogger<GetPlayerService> _logger;
    private readonly IGetPlayerRepository _repository;

    public GetPlayerService(
        ILogger<GetPlayerService> logger,
        IGetPlayerRepository repository
        )
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<int> GetPlayer(string username)
    {
        var validUsername = new Username(username);

        var playerId = await _repository.GetPlayer(validUsername);
        if (playerId == null)
        {
            throw new KeyNotFoundException($"No player with username={username} exists.");
        }
        return playerId.Value;
    }
}