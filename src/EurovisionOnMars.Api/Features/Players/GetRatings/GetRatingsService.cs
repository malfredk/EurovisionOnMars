using EurovisionOnMars.Dto.Players.PlayerRatings;
using EurovisionOnMars.Entity.Players.PlayerRatings;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.Players.GetRatings;

public interface IGetRatingsService
{
    Task<ImmutableList<PlayerRatingDto>> GetRatingsByPlayerId(int playerId);
}

public class GetRatingsService : IGetRatingsService
{
    private readonly ILogger<GetRatingsService> _logger;
    private readonly IGetRatingsRepository _repository;
    private readonly IPlayerRatingMapper _mapper;

    public GetRatingsService(
        ILogger<GetRatingsService> logger,
        IGetRatingsRepository repository,
        IPlayerRatingMapper mapper
        )
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ImmutableList<PlayerRatingDto>> GetRatingsByPlayerId(int playerId)
    {
        var ratings = await _repository.GetRatingsByPlayerId(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"There are no ratings accosiated with player id={playerId}.");
        }
        return ToOrderedDtos(ratings);
    }

    private ImmutableList<PlayerRatingDto> ToOrderedDtos(ImmutableList<PlayerRating> ratings)
    {
        return ratings
            .OrderBy(r => r.Prediction.GetPredictedRank() is null)
            .ThenBy(r => r.Prediction.GetPredictedRank()?.Value)
            .ThenBy(r => r.Country!.Number.Value)
            .Select(r => _mapper.ToDto(r))
            .ToImmutableList();
    }
}