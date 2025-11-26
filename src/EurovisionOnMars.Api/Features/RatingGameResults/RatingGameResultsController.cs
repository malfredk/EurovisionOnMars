using EurovisionOnMars.Api.Features.Common;
using EurovisionOnMars.Api.Features.PlayerGameResults;
using EurovisionOnMars.Dto.PlayerGameResults;
using EurovisionOnMars.Dto.RatingGameResults;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.RatingGameResults;

[Route("api/[controller]")]
[ApiController]
public class RatingGameResultsController : ControllerBase
{
    private readonly IRatingGameResultService _service;
    private readonly ILogger<RatingGameResultsController> _logger;
    private readonly IRatingGameResultMapper _mapper;

    public RatingGameResultsController(
        IRatingGameResultService service, 
        ILogger<RatingGameResultsController> logger,
        IRatingGameResultMapper mapper
        )
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("{playerId:int}")]
    public async Task<ActionResult<IEnumerable<RatingGameResultResponseDto>>> GetRatingGameResults(int playerId)
    {
        var ratingGameResults = await _service.GetRatingGameResults(playerId);
        var ratingGameResultDtos = Utils.MapList(ratingGameResults.ToList(), _mapper.ToDto);
        return Ok(ratingGameResultDtos);
    }
}
