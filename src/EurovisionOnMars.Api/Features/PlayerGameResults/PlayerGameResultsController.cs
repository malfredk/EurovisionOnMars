using EurovisionOnMars.Api.Features.Common;
using EurovisionOnMars.Dto.PlayerGameResults;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.PlayerGameResults;

[Route("api/[controller]")]
[ApiController]
public class PlayerGameResultsController : ControllerBase
{
    private readonly IPlayerGameResultService _service;
    private readonly ILogger<PlayerGameResultsController> _logger;
    private readonly IPlayerGameResultMapper _mapper;

    public PlayerGameResultsController(
        IPlayerGameResultService service, 
        ILogger<PlayerGameResultsController> logger,
        IPlayerGameResultMapper mapper)
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerGameResultDto>>> GetPlayerGameResults()
    {
        var playerGameResults = await _service.GetPlayerGameResults();
        var playerGameResultDtos = Utils.MapList(playerGameResults.ToList(), _mapper.ToDto);
        return Ok(playerGameResultDtos);
    }
}