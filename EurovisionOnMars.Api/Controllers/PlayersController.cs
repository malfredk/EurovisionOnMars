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

    public PlayersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
    {
        var players = await _context.Players.ToListAsync();

        return Ok(players);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<Player>> GetPlayerByUsername(string username)
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

        return Ok(player);
    }

    [HttpGet("id/{id}")]
    public async Task<ActionResult<Player>> GetPlayerById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);

        if (player == null)
        {
            return NotFound();
        }

        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<Player>> CreatePlayer([FromBody] string username)
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

        var newPlayer = new Player(username, username); // TODO: id
        _context.Players.Add(newPlayer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPlayerByUsername), new { username = newPlayer.Username }, newPlayer);
    }
}
