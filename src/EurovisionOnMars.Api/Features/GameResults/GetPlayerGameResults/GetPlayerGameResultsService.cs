using EurovisionOnMars.Dto.GameResults.GetPlayerResults;
using System.Collections.Immutable;

namespace EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;

public interface IGetPlayerGameResultsService
{
    Task<ImmutableList<PlayerGameResultDto>> GetPlayerGameResults();
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

    public async Task<ImmutableList<PlayerGameResultDto>> GetPlayerGameResults()
    {
        var playerGameResults = await _repository.GetPlayerGameResults();
        return playerGameResults
            .OrderBy(p => p.Rank is null)
            .ThenBy(p => p.Rank?.Value)
            .Select(p => _mapper.ToDto(p))
            .ToImmutableList();
    }
}