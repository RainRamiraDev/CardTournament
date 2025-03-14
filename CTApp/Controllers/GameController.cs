using CTApp.Response;
using CTDataModels.Game;
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
        public async Task<IActionResult> ResolveGame([FromBody] TournamentRequestToResolveDto request)
        {

            if (!Request.Headers.ContainsKey("X-TimeZone"))
            {
                return BadRequest(ApiResponse<IEnumerable<TournamentsInformationDto>>.ErrorResponse("La zona horaria es requerida."));
            }

            var timeZoneId = Request.Headers["X-TimeZone"].ToString();


            if (request == null)
                return BadRequest("Invalid request data.");

            GameResultDto result = await _gameService.ResolveGameAsync(request, timeZoneId);

            var response = ApiResponse<GameResultDto>.SuccessResponse("Juego resuelto exitosamente.", result);
            return Ok(response);
        }


        [Authorize(Roles = "1")]
        [HttpGet("TournamentSchedule")]
        public async Task<IActionResult> TournamentSchedule([FromBody] TournamentRequestToResolveDto request)
        {
            if (!Request.Headers.ContainsKey("X-TimeZone"))
            {
                return BadRequest(ApiResponse<IEnumerable<TournamentsInformationDto>>.ErrorResponse("La zona horaria es requerida."));
            }

            var timeZoneId = Request.Headers["X-TimeZone"].ToString();

            if (request == null)
                return BadRequest("Invalid request data.");

            List<MatchScheduleDto> result = await _gameService.CalculateMatchScheduleAsync(request, timeZoneId);

            var response = ApiResponse<List<MatchScheduleDto>>.SuccessResponse("Fechas calculadas exitosamente.", result);
            return Ok(response);
        }




    }

}
