using EurovisionOnMars.Api.Features.GameResults.CalculateGameResults;
using EurovisionOnMars.Api.Features.GameResults.GetPlayerGameResults;
using EurovisionOnMars.Dto.GameResults.GetPlayerResults;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.GameResults;

[Route("api/[controller]")]
[ApiController]
public class GameResultsController : ControllerBase
{
    private readonly ILogger<GameResultsController> _logger;
    private readonly ICalculateGameResultsService _calculateGameResultsService;
    private readonly IGetPlayerGameResultsService _getPlayerGameResultsService;

    public GameResultsController(
        ILogger<GameResultsController> logger,
        ICalculateGameResultsService calculateGameResultsService,
        IGetPlayerGameResultsService getPlayerGameResultsService
        )
    {
        _logger = logger;
        _calculateGameResultsService = calculateGameResultsService;
        _getPlayerGameResultsService = getPlayerGameResultsService;
    }

    [HttpPost]
    public async Task<ActionResult> CalculateGameResults()
    {
        await _calculateGameResultsService.CalculateGameResults();
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerGameResultDto>>> GetPlayerGameResults()
    {
        var playerGameResults = await _getPlayerGameResultsService.GetPlayerGameResults();
        return Ok(playerGameResults);
    }
}