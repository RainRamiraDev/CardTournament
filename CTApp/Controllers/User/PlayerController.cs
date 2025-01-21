using CTApp.Response;
using CTDto.Tournaments;
using CTDto.Users.Judge;
using CTService.Interfaces.Tournaments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;


        public PlayerController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }


        //[Authorize(Roles = "1")] // Solo los usuarios players puede ver
        [HttpGet("GetTournaments")]
        public async Task<IActionResult> GetTournaments()
        {
            var tournaments = await _tournamentService.GetAllTournamentAsync();

            if (tournaments is null || !tournaments.Any())
                return NotFound(ApiResponse<IEnumerable<TournamentDto>>.ErrorResponse("Torneos no encontrados."));

            return Ok(ApiResponse<IEnumerable<TournamentDto>>.SuccessResponse("torneos obtenidos exitosamente.", tournaments));
        }


        //[Authorize(Roles = "1")] // Solo los usuarios organizadores puede ver
        [HttpGet("GetAvailableTournaments")]
        public async Task<IActionResult> GetAvailableTournaments()
        {
            var tournaments = await _tournamentService.GetAllAvailableTournamentsAsync();

            if (tournaments is null || !tournaments.Any())
                return NotFound(ApiResponse<IEnumerable<AvailableTournamentsDto>>.ErrorResponse("Torneos no encontrados."));

            return Ok(ApiResponse<IEnumerable<AvailableTournamentsDto>>.SuccessResponse("Torneos obtenidos exitosamente.", tournaments));
        }


    }
}
