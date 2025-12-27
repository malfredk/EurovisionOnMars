using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Predictions;

[Route("api/[controller]")]
[ApiController]
public class PredictionsController : ControllerBase
{
    private readonly ILogger<PredictionsController> _logger;
    private readonly IPredictionServie _service;

    public PredictionsController(ILogger<PredictionsController> logger, IPredictionServie service)
    {
        _logger = logger;
        _service = service;
    }


    [HttpPatch("{id:int}")]
    public async Task<ActionResult> UpdatePrediciton(int id, [FromBody] int tieBreakDemotion)
    {
        await _service.UpdatePrediction(id, tieBreakDemotion);
        return Ok();
    }
}
