using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _service;
    private readonly ILogger<RatingsController> _logger;
    private readonly IRatingMapper _mapper;

    public RatingsController
        (
        IRatingService service,
        ILogger<RatingsController> logger,
        IRatingMapper mapper
        )
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("{playerId:int}")]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatings(int playerId)
    {
        var ratings = await _service.GetRatingsByPlayer(playerId);
        var ratingDtos = Utils.MapList(ratings.ToList(), _mapper.ToDto);
        return Ok(ratingDtos);
    }

    [HttpGet("single/{id:int}")]
    public async Task<ActionResult<RatingDto>> GetRating(int id)
    {
        var rating = await _service.GetRating(id);
        var ratingDto = _mapper.ToDto(rating);
        return Ok(ratingDto);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateRating([FromBody] RatingDto ratingDto)
    {
        var rating = await _service.GetRating(ratingDto.Id);
        var mappedRating = _mapper.UpdateEntity(rating, ratingDto); 
        await _service.UpdateRating(mappedRating);
        return Ok();
    }
}