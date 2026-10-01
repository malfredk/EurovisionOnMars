using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Api.Features.Players.GetPlayer;

public interface ICreatePlayerService
{
    Task<Player> GetPlayer(string username);
    Task<Player> CreatePlayer(string username);
}

public class CreatePlayerService : ICreatePlayerService
{
    private readonly ICreatePlayerRepository _playerRepository;
    private readonly ICountryService _countryService;
    private readonly ILogger<CreatePlayerService> _logger;

    public CreatePlayerService(
        ICreatePlayerRepository playerRepository, 
        ICountryService countryService, 
        ILogger<CreatePlayerService> logger
        )
    {
        _playerRepository = playerRepository;
        _countryService = countryService;
        _logger = logger;
    }

    public async Task<Player> GetPlayer(string username)
    {
        new Username(username); // TODO

        var player = await _playerRepository.GetPlayer(username);
        if (player == null)
        {
            throw new KeyNotFoundException($"No player with username={username} exists.");
        }
        return player;
    }

    public async Task<Player> CreatePlayer(string username)
    {
        await EnsureNewUsername(username);

        var player = await CreateEntity(username);
        return await _playerRepository.CreatePlayer(player);
    }

    private async Task EnsureNewUsername(string username)
    {
        new Username(username); // TODO

        var existingPlayer = await _playerRepository.GetPlayer(username);
        if (existingPlayer != null)
        {
            throw new DuplicateUsernameException($"Player with username={username} already exists.");
        }
    }

    private async Task<Player> CreateEntity(string username)
    {
        var countries = await _countryService.GetCountries();
        return new Player(new Username(username), countries);
    }
}