using EurovisionOnMars.Api.Repositories;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace EurovisionOnMars.Api.Services;

public interface IPlayerService
{
    Task<ImmutableList<Player>> GetPlayers();
    Task<Player> GetPlayer(int id);
    Task<Player> GetPlayer(string username);
    Task<Player> CreatePlayer(string username);
}

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IPlayerRepository repository, ILogger<PlayerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ImmutableList<Player>> GetPlayers()
    {
        return await _repository.GetPlayers();
    }

    public async Task<Player> GetPlayer(int id)
    {
        var player = await _repository.GetPlayer(id);
        if (player == null)
        {
            throw new KeyNotFoundException($"No player with id={id} exists");
        }
        return player;
    }

    public async Task<Player> GetPlayer(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username cannot be null nor empty");
        }

        var player = await _repository.GetPlayer(username);
        if (player == null)
        {
            throw new KeyNotFoundException($"No player with username={username} exists");
        }
        return player;
    }

    public async Task<Player> CreatePlayer(string username)
    {
        ValidateUsername(username);

        var existingPlayer = await _repository.GetPlayer(username);
        if (existingPlayer != null)
        {
            throw new DuplicateUsernameException($"Player with username={username} already exists");
        }
        return await _repository.CreatePlayer(username);
    }

    private void ValidateUsername(string username)
    {
        string pattern = @"^[a-zA-Z0-9æøåÆØÅ]*$";
        int maxLength = 12;

        var isValid = !string.IsNullOrEmpty(username) 
            && Regex.IsMatch(username, pattern)
            && username.Length <= maxLength;

        if (!isValid)
        {
            throw new ArgumentException("Username can only contain letters and numbers");
        }
    }
}