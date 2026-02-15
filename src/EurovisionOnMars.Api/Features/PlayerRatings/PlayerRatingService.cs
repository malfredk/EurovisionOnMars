using EurovisionOnMars.Api.Features.PlayerRatings.Domain;
using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.PlayerRatings;

public interface IPlayerRatingService
{
    Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings();
    Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId);
    Task UpdatePlayerRating(int id, UpdatePlayerRatingRequestDto ratingRequestDto);
}

public class PlayerRatingService : IPlayerRatingService
{
    private readonly IPlayerRatingRepository _repository;
    private readonly IRatingTimeValidator _ratingTimeValidator;
    private readonly ILogger<PlayerRatingService> _logger;
    private readonly IPlayerRatingProcessor _playerRatingProcessor;

    public PlayerRatingService(
        IPlayerRatingRepository repository,
        IRatingTimeValidator ratingTimeValidator,
        ILogger<PlayerRatingService> logger,
        IPlayerRatingProcessor playerRatingProcessor
        )
    {
        _repository = repository;
        _ratingTimeValidator = ratingTimeValidator;
        _logger = logger;
        _playerRatingProcessor = playerRatingProcessor;
    }

    public async Task<IReadOnlyList<PlayerRating>> GetAllPlayerRatings()
    {
        return await _repository.GetAllPlayerRatings();
    }

    public async Task<ImmutableList<PlayerRating>> GetPlayerRatingsByPlayerId(int playerId)
    {
        var ratings = await _repository.GetPlayerRatingsByPlayerId(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"Player with id={playerId} is missing ratings.");
        }
        return SortRatings(ratings);
    }

    public async Task UpdatePlayerRating(int id, UpdatePlayerRatingRequestDto ratingRequestDto)
    {
        _ratingTimeValidator.EnsureRatingIsOpen();

        var ratings = await _repository.GetPlayerRatingsForPlayer(id);
        var editedRating = ratings.First(r => r.Id == id);

        _playerRatingProcessor.UpdatePlayerRating(ratingRequestDto, editedRating, ratings);

        await _repository.SaveChanges();
    }

    private ImmutableList<PlayerRating> SortRatings(ImmutableList<PlayerRating> ratings)
    {
        return ratings
            .OrderBy(r => r.Prediction.GetPredictedRank() ?? 100)
            .ThenBy(r => r.Country.Number)
            .ToImmutableList();
    }
}