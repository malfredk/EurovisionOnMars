using EurovisionOnMars.Api.Features.Common;
using EurovisionOnMars.Dto.PlayerGameResults;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

[Route("api/[controller]")]
[ApiController]
public class GameResultsController : ControllerBase
{
    private readonly IPlayerGameResultService _service;
    private readonly ILogger<GameResultsController> _logger;
    private readonly IPlayerGameResultMapper _mapper = new PlayerGameResultMapper();

    public GameResultsController(IPlayerGameResultService service, ILogger<GameResultsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerGameResultResponseDto>>> GetPlayerGameResults()
    {
        var playerGameResults = await _service.GetPlayerGameResults();
        var playerGameResultDtos = Utils.MapList(playerGameResults.ToList(), _mapper.ToDto);
        return Ok(playerGameResultDtos);
    }
}