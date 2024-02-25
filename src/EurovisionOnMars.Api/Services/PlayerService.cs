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
    private readonly IPlayerRepository _playerRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(
        IPlayerRepository playerRepository, 
        ICountryRepository countryRepository, 
        ILogger<PlayerService> logger
        )
    {
        _playerRepository = playerRepository;
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<ImmutableList<Player>> GetPlayers()
    {
        return await _playerRepository.GetPlayers();
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
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username cannot be null nor empty");
        }

        var player = await _playerRepository.GetPlayer(username);
        if (player == null)
        {
            throw new KeyNotFoundException($"No player with username={username} exists");
        }
        return player;
    }

    public async Task<Player> CreatePlayer(string username)
    {
        ValidateUsername(username);

        var existingPlayer = await _playerRepository.GetPlayer(username);
        if (existingPlayer != null)
        {
            throw new DuplicateUsernameException($"Player with username={username} already exists");
        }

        var ratings = await CreateInitialRatings();
        var player = new Player 
        { 
            Username = username,
            Ratings = ratings
        };
        return await _playerRepository.CreatePlayer(player);
    }

    private async Task<List<Rating>> CreateInitialRatings()
    {
        var ratings = new List<Rating>();
        var countries = await _countryRepository.GetCountries();
        foreach (var country in countries)
        {
            ratings.Add(CreateInitialRating(country));
        }
        return ratings;
    }

    private Rating CreateInitialRating(Country country)
    {
        return new Rating
        {
            CountryId = country.Id,
            Country = country
        };
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