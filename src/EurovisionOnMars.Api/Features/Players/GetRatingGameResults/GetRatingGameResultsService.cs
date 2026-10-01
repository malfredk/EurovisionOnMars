using EurovisionOnMars.Dto.RatingGameResults;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.Players.GetRatingGameResults;

public interface IGetRatingGameResultsService
{
    Task<ImmutableList<RatingGameResultDto>> GetRatingGameResults(int playerId);
}

public class GetRatingGameResultsService : IGetRatingGameResultsService
{
    private readonly ILogger<GetRatingGameResultsService> _logger;
    private readonly IGetRatingGameResultsRepository _repository;
    private readonly IRatingGameResultMapper _mapper;

    public GetRatingGameResultsService
        (
        ILogger<GetRatingGameResultsService> logger,
        IGetRatingGameResultsRepository repository,
        IRatingGameResultMapper mapper
        )
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ImmutableList<RatingGameResultDto>> GetRatingGameResults(int playerId)
    {
        var ratings = await _repository.GetRatings(playerId);
        return ratings
            .Select(rating => _mapper.ToDto(rating))
            .ToImmutableList();
    }
}