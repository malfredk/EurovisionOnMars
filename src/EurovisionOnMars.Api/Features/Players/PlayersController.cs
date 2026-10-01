using EurovisionOnMars.Api.Features.Players.CreatePlayer;
using EurovisionOnMars.Api.Features.Players.GetPlayer;
using EurovisionOnMars.Dto.PlayerRatings;
using EurovisionOnMars.Dto.Players;
using EurovisionOnMars.Dto.Predictions;
using EurovisionOnMars.Dto.RatingGameResults;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Players;

[Route("api/players")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly ILogger<PlayersController> _logger;
    private readonly ICreatePlayerService _createPlayerService;
    private readonly IGetPlayerService _getPlayerService;

    public PlayersController(
        ILogger<PlayersController> logger,
        ICreatePlayerService createPlayerService,
        IGetPlayerService getPlayerService
        )
    {
        _logger = logger;
        _createPlayerService = createPlayerService;
        _getPlayerService = getPlayerService;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<PlayerDto>> GetPlayer(string username)
    {
        var player = await _getPlayerService.GetPlayer(username);
        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePlayer([FromBody] string username)
    {
        await _createPlayerService.CreatePlayer(username);

        return CreatedAtAction(
            nameof(GetPlayer),
            new { username = username },
            value: null);
    }

    [HttpGet("{playerId:int}/ratings")]
    public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings(
        int playerId)
    {
        ...
    }

    [HttpPatch("{playerId:int}/ratings/{ratingId:int}")]
    public async Task<ActionResult> UpdatePlayerRating(
        int playerId,
        int ratingId,
        [FromBody] UpdatePlayerRatingRequestDto request)
    {
        ...
    }

    [HttpGet("{playerId:int}/rating-results")]
    public async Task<ActionResult<IEnumerable<RatingGameResultDto>>> GetRatingResults(
        int playerId)
    {
        ...
    }

    [HttpPatch("{playerId:int}/tie-break-demotions")]
    public async Task<ActionResult> ResolveTieBreak(
        int playerId,
        [FromBody] ResolveTieBreakRequestDto request)
    {
        ...
    }
}
