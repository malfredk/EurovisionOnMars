using EurovisionOnMars.Api.Mappers;
using EurovisionOnMars.Dto;
using EurovisionOnMars.Entity;
using EurovisionOnMars.Entity.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EurovisionOnMars.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly DataContext _context;
    private readonly PlayerMapper _mapper = new PlayerMapper(); // TODO: consider dependency injection

    public PlayersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
    {
        var players = await _context.Players.ToListAsync();
        var playerDtos = players.Select(player => _mapper.ToDto(player));
        return Ok(playerDtos);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<PlayerDto>> GetPlayerByUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest();
        }

        var player = await _context.Players.FirstOrDefaultAsync(p => p.Username == username);

        if (player == null)
        {
            return NotFound();
        }

        var playerDto = _mapper.ToDto(player);
        return Ok(playerDto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerDto>> GetPlayerById(int id)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);

        if (player == null)
        {
            return NotFound();
        }

        var playerDto = _mapper.ToDto(player);
        return Ok(playerDto);
    }

    [HttpPost("{username}")]
    public async Task<ActionResult<PlayerDto>> CreatePlayer(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest();
        }

        var existingPlayer = await _context.Players.FirstOrDefaultAsync(p => p.Username == username);
        if (existingPlayer != null)
        {
            return Conflict();
        }

        var newPlayer = new Player(username);
        _context.Players.Add(newPlayer);
        await _context.SaveChangesAsync();

        var newPlayerDto = _mapper.ToDto(newPlayer);

        return CreatedAtAction(nameof(GetPlayerByUsername), new { username = newPlayer.Username }, newPlayerDto);
    }
}
