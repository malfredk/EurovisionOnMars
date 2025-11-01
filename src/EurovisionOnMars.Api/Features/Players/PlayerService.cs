using EurovisionOnMars.Api.Features.Countries;
using EurovisionOnMars.CustomException;
using EurovisionOnMars.Entity;
using System.Text.RegularExpressions;

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
        ValidateUsername(username);

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

        var player = await CreateEntity(username);
        return await _playerRepository.CreatePlayer(player);
    }

    public async Task<Player> CreateEntity(string username)
    {
        return new Player
        {
            Username = username,
            PlayerRatings = await CreateInitialRatings(),
            PlayerGameResult = new PlayerGameResult()
        };
    }

    private async Task<List<PlayerRating>> CreateInitialRatings()
    {
        var ratings = new List<PlayerRating>();
        var countries = await _countryService.GetCountries();
        foreach (var country in countries)
        {
            ratings.Add(CreateInitialRating(country));
        }
        return ratings;
    }

    private PlayerRating CreateInitialRating(Country country)
    {
        return new PlayerRating
        {
            CountryId = country.Id,
            RatingGameResult = new RatingGameResult(),
            Prediction = new Prediction()
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