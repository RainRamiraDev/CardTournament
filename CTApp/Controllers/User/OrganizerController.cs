using CTApp.Response;
using CTDataModels.Tournamets;
using CTDto.Tournaments;
using CTDto.Users;
using CTDto.Users.Judge;
using CTService.Implementation.User;
using CTService.Interfaces.Tournaments;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.User
{


    [ApiController]
    [Route("api/[controller]")]

    public class OrganizerController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly ITournamentService _tournamentService;

        public OrganizerController(IUserService userService, ITournamentService tournamentService)
        {
            _userService = userService;
            _tournamentService = tournamentService;
        }


        [Authorize(Roles = "1")] // Solo los usuarios organizadores puede ver
        [HttpGet("GetJudges")]
        public async Task<IActionResult> GetJudges()
        {
            var judges = await _userService.GetAllJudgesAsync();

            if (judges is null || !judges.Any())
                return NotFound(ApiResponse<IEnumerable<JudgeDto>>.ErrorResponse("Jueces no encontrados."));

            return Ok(ApiResponse<IEnumerable<JudgeDto>>.SuccessResponse("Jueces obtenidos exitosamente.", judges));
        }



        [Authorize(Roles = "1")]
        [HttpPost("CreateTournament")]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentDto tournamentDto)
        {
            if (tournamentDto == null)
            {
                return BadRequest("Invalid tournament data.");
            }

            var id = await _tournamentService.CreateTournamentAsync(tournamentDto);

            if (id == 0)
            {
                return StatusCode(500, "Error creating tournament.");
            }

            return Created("", new { id });
        }


        [Authorize(Roles = "1")]
        [HttpPost("SetTournamentJudges")]
        public async Task<IActionResult> SetTournamentJudges([FromBody] TournamentJudgeDto tournamentJudgeDto)
        {
            try
            {
                var affectedRows = await _tournamentService.InsertTournamentJudgesAsync(tournamentJudgeDto);

                if (affectedRows == 0)
                {
                    return StatusCode(500, "Error assigning judges to tournament.");
                }

                return Created("", new { affectedRows });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }









    }
}
