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
        [HttpPost("resolve")]
        public async Task<IActionResult> ResolveGame([FromBody] TournamentRequestDto request)
        {
            try
            {
                Console.WriteLine("id recibido: " + request.Tournament_Id);

                GameResultDto result = await _gameService.ResolveGameAsync(request.Tournament_Id);
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
