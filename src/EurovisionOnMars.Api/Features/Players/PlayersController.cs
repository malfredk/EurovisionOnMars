using EurovisionOnMars.Dto.Players;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Features.Players;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _service;
    private readonly ILogger<PlayersController> _logger;
    private readonly IPlayerMapper _mapper;

    public PlayersController(
        IPlayerService service, 
        ILogger<PlayersController> logger,
        IPlayerMapper mapper
        )
    {
        _service = service;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<PlayerDto>> GetPlayer(string username)
    {
        var player = await _service.GetPlayer(username);
        var playerDto = _mapper.ToDto(player);
        return Ok(playerDto);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> CreatePlayer([FromBody] string username)
    {
        var player = await _service.CreatePlayer(username);
        var playerDto = _mapper.ToDto(player);
        return CreatedAtAction(nameof(GetPlayer), new { username = player.Username }, playerDto);
    }
}