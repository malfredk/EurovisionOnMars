using EurovisionOnMars.Dto.Players.ResolveTieBreak;
using EurovisionOnMars.Entity.Players;

namespace EurovisionOnMars.Api.Features.Players.ResolveTieBreak;

public interface IResolveTieBreakService
{
    Task UpdateTieBreakDemotions(int playerId, ResolveTieBreakRequestDto request);
}

public class ResolveTieBreakService : IResolveTieBreakService
{
    private readonly ILogger<ResolveTieBreakService> _logger;
    private readonly IRatingTimeValidator _ratingTimeValidator;
    private readonly IResolveTieBreakRepository _repository;

    public ResolveTieBreakService(
        ILogger<ResolveTieBreakService> logger,
        IRatingTimeValidator ratingTimeValidator,
        IResolveTieBreakRepository repository
        )
    {
        _logger = logger;
        _ratingTimeValidator = ratingTimeValidator;
        _repository = repository;
    }

    public async Task UpdateTieBreakDemotions(int playerId, ResolveTieBreakRequestDto request)
    {
        _ratingTimeValidator.EnsureRatingIsOpen();

        var player = await GetPlayer(playerId);

        player.ResolveTieBreak(request.OrderedPredictionIds);

        await _repository.SaveChanges();
    }

    private async Task<Player> GetPlayer(int playerId)
    {
        var player = await _repository.GetPlayer(playerId);
        if (player == null)
            throw new KeyNotFoundException($"Player with id={playerId} does not exist.");
        return player;
    }
}
