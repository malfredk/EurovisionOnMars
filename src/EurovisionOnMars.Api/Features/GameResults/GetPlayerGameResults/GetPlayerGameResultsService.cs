using EurovisionOnMars.Dto.GameResults.GetPlayerResults;

namespace EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;

public interface IGetPlayerGameResultsService
{
    Task<IReadOnlyList<PlayerGameResultDto>> GetPlayerGameResults();
}

public class GetPlayerGameResultsService : IGetPlayerGameResultsService
{
    private readonly ILogger<GetPlayerGameResultsService> _logger;
    private readonly IGetPlayerGameResultsRepository _repository;
    private readonly IPlayerGameResultMapper _mapper;

    public GetPlayerGameResultsService
        (
        ILogger<GetPlayerGameResultsService> logger,
        IGetPlayerGameResultsRepository repository,
        IPlayerGameResultMapper mapper
        )
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PlayerGameResultDto>> GetPlayerGameResults()
    {
        var playerGameResults = await _repository.GetPlayerGameResults();
        return playerGameResults
            .Select(p => _mapper.ToDto(p))
            .OrderBy(p => p.Rank is null)
            .ThenBy(p => p.Rank)
            .ToList();
    }
}