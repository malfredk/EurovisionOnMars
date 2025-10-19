using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Dto.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _service;
    private readonly ILogger<RatingsController> _logger;
    private readonly IPlayerRatingMapper _mapper;

    public RatingsController
        (
        IRatingService service,
        ILogger<RatingsController> logger,
        IPlayerRatingMapper mapper
        )
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("{playerId:int}")]
    public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetRatings(int playerId)
    {
        var ratings = await _service.GetRatingsByPlayer(playerId);
        var ratingDtos = Utils.MapList(ratings.ToList(), _mapper.ToDto);
        return Ok(ratingDtos);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult> UpdateRating(int id, [FromBody] RatingPointsRequestDto ratingRequestDto)
    {
        await _service.UpdateRating(id, ratingRequestDto);
        return Ok();
    }

    [HttpPatch("{id:int}/Rank")]
    public async Task<ActionResult> UpdateRating(int id, [FromBody] int rank)
    {
        await _service.UpdateRating(id, rank);
        return Ok();
    }
}