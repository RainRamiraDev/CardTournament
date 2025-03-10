using CTApp.Response;
using CTDataModels.Users.Judge;
using CTDto.Tournaments;
using CTDto.Users.Judge;
using CTService.Interfaces.Card;
using CTService.Interfaces.Tournaments;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class JudgeController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ICardService _cardService;

        public JudgeController(IUserService userService, ITournamentService tournamentService, ICardService cardService)
        {
            _userService = userService;
            _tournamentService = tournamentService;
            _cardService = cardService;
        }

        //Puede descalificar un jugador si es necesario. por endpoint

        [Authorize(Roles = "3")]
        [HttpPost("DisqualifyPlayerFromTournament")]
        public async Task<IActionResult> DisqualifyPlayerFromTournament([FromBody] DisqualificationDto disqualificationDto)
        {
            if (disqualificationDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Informacion de descalificacion incorrecta."));

            await _tournamentService.DisqualifyPlayerFromTournamentAsync(disqualificationDto);

            return Created("", ApiResponse<object>.SuccessResponse("Jugador eliminado correctamente."));
        }




        //puede ver los jugadores de un torneo determinado  por endpoint

        [Authorize(Roles = "3")]
        [HttpGet("ShowPlayersFromTournament")]
        public async Task<IActionResult> ShowPlayersFromTournament([FromBody] TournamentRequestToResolveDto showPlayersFromTournamentDto)
        {
            Console.WriteLine("[INFO]: Id del torneo = " + showPlayersFromTournamentDto.Tournament_Id);


            if (showPlayersFromTournamentDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Información del torneo incorrecta."));

            var players = await _tournamentService.ShowPlayersFromTournamentAsync(showPlayersFromTournamentDto);

            if (players == null || players.Count == 0)
                return NotFound(ApiResponse<object>.ErrorResponse("No se encontraron jugadores para el torneo especificado."));

            return Ok(ApiResponse<List<ShowTournamentPlayersDto>>.SuccessResponse("Jugadores encontrados exitosamente.", players));
        }


    }
}
