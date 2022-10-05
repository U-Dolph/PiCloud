using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiCloud.Data;
using PiCloud.Models;
using System.Data;

namespace PiCloudAPI.Controllers
{
    [Route("admin-api")]
    [ApiController]
    public class AdminGamesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminGamesController(AppDbContext context) => _context = context;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [Route("games")]
        public async Task<IEnumerable<Game>> Get()
        {
            return await _context.Games.ToListAsync();
        }

        [HttpGet("game/{id}")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _context.Games.FindAsync(id);
            return game == null ? NotFound() : Ok(game);
        }

        [HttpPost("add-game")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddGame(Game game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
        }

        [HttpPatch("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateGame(int id, Game game)
        {
            if (id != game.Id) return BadRequest();

            _context.Entry(game).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var gameToDelete = await _context.Games.FindAsync(id);

            if (gameToDelete == null) return NotFound();

            _context.Games.Remove(gameToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
