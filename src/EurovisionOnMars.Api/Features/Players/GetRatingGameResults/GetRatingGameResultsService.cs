using EurovisionOnMars.Dto.Players.GetRatingGameResults;

namespace EurovisionOnMars.Api.Features.Players.GetRatingGameResults;

public interface IGetRatingGameResultsService
{
    Task<IReadOnlyList<RatingGameResultDto>> GetRatingGameResults(int playerId);
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

    public async Task<IReadOnlyList<RatingGameResultDto>> GetRatingGameResults(int playerId)
    {
        var ratings = await _repository.GetRatings(playerId);
        return ratings
            .Select(rating => _mapper.ToDto(rating))
            .OrderBy(r => r.Country.ActualRank)
            .ToList();
    }
}