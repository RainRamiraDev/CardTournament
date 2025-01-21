using CTApp.Response;
using CTDto.Card;
using CTDto.Tournaments;
using CTDto.Users.Judge;
using CTService.Interfaces.Card;
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
        private readonly ICardService _cardService;


        public PlayerController(ITournamentService tournamentService, ICardService cardService)
        {
            _tournamentService = tournamentService;
            _cardService = cardService;
        }


        [Authorize(Roles = "4")] 
        [HttpGet("GetTournaments")]
        public async Task<IActionResult> GetTournaments()
        {
            var tournaments = await _tournamentService.GetAllTournamentAsync();

            if (tournaments is null || !tournaments.Any())
                return NotFound(ApiResponse<IEnumerable<TournamentDto>>.ErrorResponse("Torneos no encontrados."));

            return Ok(ApiResponse<IEnumerable<TournamentDto>>.SuccessResponse("torneos obtenidos exitosamente.", tournaments));
        }


        [Authorize(Roles = "4")] 
        [HttpGet("GetAvailableTournaments")]
        public async Task<IActionResult> GetAvailableTournaments()
        {
            var tournaments = await _tournamentService.GetAllAvailableTournamentsAsync();

            if (tournaments is null || !tournaments.Any())
                return NotFound(ApiResponse<IEnumerable<AvailableTournamentsDto>>.ErrorResponse("Torneos no encontrados."));

            return Ok(ApiResponse<IEnumerable<AvailableTournamentsDto>>.SuccessResponse("Torneos obtenidos exitosamente.", tournaments));
        }


        [Authorize(Roles = "4")]
        [HttpGet("ShowCards")]
        public async Task<IActionResult> GetAllCards()
        {
            try
            {
                var cards = await _cardService.GetAllCardsAsync();

                if (cards == null || !cards.Any())
                {
                    return NotFound(ApiResponse<IEnumerable<ShowCardsDto>>.ErrorResponse("Cartas no encontradas."));
                }

                var response = ApiResponse<IEnumerable<ShowCardsDto>>.SuccessResponse("Cartas obtenidas exitosamente.", cards);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errors = new List<string> { "Ocurrió un error al obtener las cartas." };
                var stackTrace = ex.StackTrace;
                var response = ApiResponse<ShowCardsDto>.ErrorResponse(errors, stackTrace);
                return BadRequest(response);
            }
        }

    }
}
