using EurovisionOnMars.Dto.Predictions;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Players.ResolveTieBreak;

[Route("api/[controller]")]
[ApiController]
public class PredictionsController : ControllerBase
{
    private readonly ILogger<PredictionsController> _logger;
    private readonly IResolveTieBreakService _service;

    public PredictionsController(ILogger<PredictionsController> logger, IResolveTieBreakService service)
    {
        _logger = logger;
        _service = service;
    }


    [HttpPatch("TieBreakDemotions")]
    public async Task<ActionResult> UpdateTieBreakDemotions([FromBody] ResolveTieBreakRequestDto request)
    {
        await _service.UpdateTieBreakDemotions(request);
        return Ok();
    }
}
