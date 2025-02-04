using CTDto.Game;
using CTDto.Tournaments;
using CTService.Interfaces.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }


        [Authorize(Roles = "1")]
        [HttpPost("InsertGamePlayers")]
        public async Task<IActionResult> InsertGamePlayers([FromBody] GamePlayersDto gamePlayersDto)
        {
            if (gamePlayersDto == null)
            {
                return BadRequest("Invalid game data.");
            }

            var id = await _gameService.InsertGamePlayersAsync(gamePlayersDto);

            if (id == 0)
            {
                return StatusCode(500, "Error creating game.");
            }

            return Created("", new { id });
        }

        [Authorize(Roles = "1")]
        [HttpPost("resolve")]
        public async Task<IActionResult> ResolveGame()
        {
            try
            {
                GameResultDto result = await _gameService.ResolveGameAsync();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor.", error = ex.Message });
            }
        }

    }
}
