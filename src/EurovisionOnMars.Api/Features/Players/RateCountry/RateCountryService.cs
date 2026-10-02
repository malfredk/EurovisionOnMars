using EurovisionOnMars.Dto.Players.RateCountry;
using EurovisionOnMars.Entity.Players;
using EurovisionOnMars.Entity.Players.PlayerRatings;

namespace EurovisionOnMars.Api.Features.Players.RateCountry;

public interface IRateCountryService
{
    Task RateCountry(int playerId, int ratingId, RateCountryRequestDto ratingRequestDto);
}

public class RateCountryService : IRateCountryService
{
    private readonly ILogger<RateCountryService> _logger;
    private readonly IRateCountryRepository _repository;
    private readonly IRatingTimeValidator _ratingTimeValidator;

    public RateCountryService(
        ILogger<RateCountryService> logger,
        IRateCountryRepository repository,
        IRatingTimeValidator ratingTimeValidator
        )
    {
        _logger = logger;
        _repository = repository;
        _ratingTimeValidator = ratingTimeValidator;
    }

    public async Task RateCountry(int playerId, int ratingId, RateCountryRequestDto ratingRequestDto)
    {
        _ratingTimeValidator.EnsureRatingIsOpen();

        var player = await GetPlayer(playerId);

        RateCountry(player, ratingId, ratingRequestDto);

        await _repository.SaveChanges();
    }

    private async Task<Player> GetPlayer(int playerId)
    {
        var player = await _repository.GetPlayer(playerId);
        if (player == null)
        {
            throw new ArgumentException($"There is no player with id {playerId}.");
        }
        return player;
    }

    private void RateCountry(Player player, int ratingId, RateCountryRequestDto ratingRequestDto)
    {
        var category1Points = new Points(ratingRequestDto.Category1Points);
        var category2Points = new Points(ratingRequestDto.Category2Points);
        var category3Points = new Points(ratingRequestDto.Category3Points);

        player.RateCountry(ratingId, category1Points, category2Points, category3Points);
    }
}