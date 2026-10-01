using EurovisionOnMars.Api.Features.Players.RateCountry.Domain;
using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.Players.RateCountry;

public interface IRateCountryService
{
    Task UpdatePlayerRating(int id, UpdatePlayerRatingRequestDto ratingRequestDto);
}

public class RateCountryService : IRateCountryService
{
    private readonly ILogger<RateCountryService> _logger;
    private readonly IRateCountryRepository _repository;
    private readonly IRatingTimeValidator _ratingTimeValidator;
    private readonly IPlayerRatingProcessor _playerRatingProcessor;

    public RateCountryService(
        ILogger<RateCountryService> logger,
        IRateCountryRepository repository,
        IRatingTimeValidator ratingTimeValidator,
        IPlayerRatingProcessor playerRatingProcessor
        )
    {
        _logger = logger;
        _repository = repository;
        _ratingTimeValidator = ratingTimeValidator;
        _playerRatingProcessor = playerRatingProcessor;
    }

    public async Task UpdatePlayerRating(int id, UpdatePlayerRatingRequestDto ratingRequestDto)
    {
        _ratingTimeValidator.EnsureRatingIsOpen();

        var ratings = await _repository.GetPlayerRatingsForPlayer(id);
        var ratingToUpdate = ratings.First(r => r.Id == id);

        _playerRatingProcessor.UpdatePlayerRating(ratingRequestDto, ratingToUpdate, ratings);

        await _repository.SaveChanges();
    }
}