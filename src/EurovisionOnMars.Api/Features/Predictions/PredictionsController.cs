using EurovisionOnMars.Dto.Predictions;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Predictions;

[Route("api/[controller]")]
[ApiController]
public class PredictionsController : ControllerBase
{
    private readonly ILogger<PredictionsController> _logger;
    private readonly IPredictionService _service;

    public PredictionsController(ILogger<PredictionsController> logger, IPredictionService service)
    {
        _logger = logger;
        _service = service;
    }


    [HttpPatch]
    public async Task<ActionResult> UpdateTieBreakDemotions([FromBody] ResolveTieBreakRequestDto request)
    {
        await _service.UpdateTieBreakDemotions(request);
        return Ok();
    }
}
