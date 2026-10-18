using EurovisionOnMars.Domain.Players.PlayerRatings;
using EurovisionOnMars.Dto.Players.GetRatings;

namespace EurovisionOnMars.Api.Features.Players.GetRatings;

public interface IGetRatingsService
{
    Task<IReadOnlyList<PlayerRatingDto>> GetRatingsByPlayerId(int playerId);
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

    public async Task<IReadOnlyList<PlayerRatingDto>> GetRatingsByPlayerId(int playerId)
    {
        var ratings = await _repository.GetRatingsByPlayerId(playerId);
        if (!ratings.Any())
        {
            throw new KeyNotFoundException($"There are no ratings accosiated with player id={playerId}.");
        }
        return ToOrderedDtos(ratings);
    }

    private IReadOnlyList<PlayerRatingDto> ToOrderedDtos(
        IReadOnlyCollection<PlayerRating> ratings)
    {
        return ratings
            .Select(r => _mapper.ToDto(r))
            .OrderBy(r => r.Prediction.PredictedRank is null)
            .ThenBy(r => r.Prediction.PredictedRank)
            .ThenBy(r => r.Country.Number)
            .ToList();
    }
}