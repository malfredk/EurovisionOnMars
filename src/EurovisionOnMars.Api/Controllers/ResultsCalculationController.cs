using EurovisionOnMars.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResultsCalculationController : ControllerBase
{
    private readonly IResultService _service;
    private readonly ILogger<ResultsCalculationController> _logger;

    public ResultsCalculationController(IResultService service, ILogger<ResultsCalculationController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> CalculateResults()
    {
        await _service.CalculateResults();
        return Ok();
    }
}