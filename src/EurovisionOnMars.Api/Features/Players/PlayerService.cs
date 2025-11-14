using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;

namespace EurovisionOnMars.Api.Features.Players;

public interface IPlayerService
{
    Task<Player> GetPlayer(int id);
    Task<Player> GetPlayer(string username);
    Task<Player> CreatePlayer(string username);
}

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ICountryService _countryService;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository playerRepository, 
        ICountryService countryService, 
        ILogger<PlayerService> logger
        )
    {
        _playerRepository = playerRepository;
        _countryService = countryService;
        _logger = logger;
    }

    public async Task<Player> GetPlayer(int id)
    {
        var player = await _playerRepository.GetPlayer(id);
        if (player == null)
        {
            throw new KeyNotFoundException($"No player with id={id} exists");
        }
        return player;
    }

    public async Task<Player> GetPlayer(string username)
    {
        Player.ValidateUsername(username);

        var player = await _playerRepository.GetPlayer(username);
        if (player == null)
        {
            throw new KeyNotFoundException($"No player with username={username} exists");
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
        Player.ValidateUsername(username);

        var existingPlayer = await _playerRepository.GetPlayer(username);
        if (existingPlayer != null)
        {
            throw new DuplicateUsernameException($"Player with username={username} already exists");
        }
    }

    private async Task<Player> CreateEntity(string username)
    {
        var countries = await _countryService.GetCountries();
        return new Player(username, countries);
    }
}