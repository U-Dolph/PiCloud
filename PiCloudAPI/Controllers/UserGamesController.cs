using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiCloud.Data;
using PiCloud.Models;

namespace PiCloudAPI.Controllers
{
    [Route("user-api")]
    [ApiController]
    public class UserGamesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserGamesController(AppDbContext context) => _context = context;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("games")]
        public async Task<IEnumerable<Game>> Get()
        {
            return await _context.Games.ToListAsync();
        }

        [HttpGet("game/{id}")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _context.Games.FindAsync(id);
            return game == null ? NotFound() : Ok(game);
        }
    }
}
