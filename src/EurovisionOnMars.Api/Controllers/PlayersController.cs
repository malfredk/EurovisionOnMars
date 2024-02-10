using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Api.Services;
using EurovisionOnMars.Dto;
using Microsoft.AspNetCore.Mvc;

namespace EurovisionOnMars.Api.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
    {
        var players = await _service.GetPlayers();
        var playerDtos = players.Select(player => _mapper.ToDto(player));
        return Ok(playerDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerDto>> GetPlayerById(int id)
    {
        var player = await _service.GetPlayer(id);
        var playerDto = _mapper.ToDto(player);
        return Ok(playerDto);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<PlayerDto>> GetPlayerByUsername(string username)
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
        return CreatedAtAction(nameof(GetPlayerByUsername), new { username = player.Username }, playerDto);
    }
}
