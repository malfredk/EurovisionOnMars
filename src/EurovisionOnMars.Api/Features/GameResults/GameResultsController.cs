using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.GameResults;

[Route("api/[controller]")]
[ApiController]
public class GameResultsController : ControllerBase
{
    private readonly IGameResultService _service;
    private readonly ILogger<GameResultsController> _logger;

    public GameResultsController(IGameResultService service, ILogger<GameResultsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CalculateGameResults()
    {
        await _service.CalculateGameResults();
        return Ok();
    }
}