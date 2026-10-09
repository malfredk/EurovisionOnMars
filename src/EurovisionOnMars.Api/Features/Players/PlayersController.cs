using EurovisionOnMars.Api.Features.Players.CreatePlayer;
using EurovisionOnMars.Api.Features.Players.GetPlayer;
using EurovisionOnMars.Api.Features.Players.GetRatingGameResults;
using EurovisionOnMars.Api.Features.Players.GetRatings;
using EurovisionOnMars.Api.Features.Players.RateCountry;
using EurovisionOnMars.Api.Features.Players.ResolveTieBreak;
using EurovisionOnMars.Dto.Players.GetPlayer;
using EurovisionOnMars.Dto.Players.GetRatingGameResults;
using EurovisionOnMars.Dto.Players.GetRatings;
using EurovisionOnMars.Dto.Players.RateCountry;
using EurovisionOnMars.Dto.Players.ResolveTieBreak;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Players;

[Route("api/players")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly ILogger<PlayersController> _logger;
    private readonly ICreatePlayerService _createPlayerService;
    private readonly IGetPlayerService _getPlayerService;
    private readonly IGetRatingsService _getRatingsService;
    private readonly IRateCountryService _rateCountryService;
    private readonly IGetRatingGameResultsService _getRatingGameResultsService;
    private readonly IResolveTieBreakService _resolveTieBreakService;

    public PlayersController(
        ILogger<PlayersController> logger,
        ICreatePlayerService createPlayerService,
        IGetPlayerService getPlayerService,
        IGetRatingsService getRatingsService,
        IRateCountryService rateCountryService,
        IGetRatingGameResultsService getRatingGameResults,
        IResolveTieBreakService resolveTieBreakService
        )
    {
        _logger = logger;
        _createPlayerService = createPlayerService;
        _getPlayerService = getPlayerService;
        _getRatingsService = getRatingsService;
        _rateCountryService = rateCountryService;
        _getRatingGameResultsService = getRatingGameResults;
        _resolveTieBreakService = resolveTieBreakService;
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
            new { username },
            value: null);
    }

    [HttpGet("{playerId:int}/ratings")]
    public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetRatings(
        int playerId)
    {
        var ratings = await _getRatingsService.GetRatingsByPlayerId(playerId);
        return Ok(ratings);
    }

    [HttpPatch("{playerId:int}/ratings/{ratingId:int}")]
    public async Task<ActionResult> RateCountry(
        int playerId,
        int ratingId,
        [FromBody] RateCountryRequestDto request)
    {
        await _rateCountryService.RateCountry(playerId, ratingId, request);
        return Ok();
    }

    [HttpGet("{playerId:int}/rating-results")]
    public async Task<ActionResult<IEnumerable<RatingGameResultDto>>> GetRatingResults(
        int playerId)
    {
        var results = await _getRatingGameResultsService.GetRatingGameResults(playerId);
        return Ok(results);
    }

    [HttpPatch("{playerId:int}/predictions/tie-break")]
    public async Task<ActionResult> ResolveTieBreak(
        int playerId,
        [FromBody] ResolveTieBreakRequestDto request)
    {
        await _resolveTieBreakService.UpdateTieBreakDemotions(playerId, request);
        return Ok();
    }
}
