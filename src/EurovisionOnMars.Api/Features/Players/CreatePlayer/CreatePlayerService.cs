using EurovisionOnMars.CustomException;
using EurovisionOnMars.Domain.Players;

namespace EurovisionOnMars.Api.Features.Players.CreatePlayer;

public interface ICreatePlayerService
{
    Task CreatePlayer(string username);
}

public class CreatePlayerService : ICreatePlayerService
{
    private readonly ILogger<CreatePlayerService> _logger;
    private readonly ICreatePlayerRepository _repository;

    public CreatePlayerService(
        ILogger<CreatePlayerService> logger,
        ICreatePlayerRepository repository
        )
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task CreatePlayer(string username)
    {
        var validUsername = await ValidateUsername(username);

        var player = await CreateEntity(validUsername);
        await _repository.AddPlayer(player);
    }

    private async Task<Username> ValidateUsername(string username)
    {
        var validUsername = new Username(username);

        var usernameExists = await _repository.UsernameExists(validUsername);
        if (usernameExists)
        {
            throw new DuplicateUsernameException($"Player with username={username} already exists.");
        }

        return validUsername;
    }

    private async Task<Player> CreateEntity(Username username)
    {
        var countries = await _repository.GetCountries();
        return new Player(username, countries);
    }
}