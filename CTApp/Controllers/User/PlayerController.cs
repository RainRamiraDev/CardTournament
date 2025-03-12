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
        private readonly ILogger<PlayerController> _logger;


        public PlayerController(ITournamentService tournamentService, ICardService cardService, ILogger<PlayerController> logger)
        {
            _tournamentService = tournamentService;
            _cardService = cardService;
            _logger = logger;
        }


        [Authorize(Roles = "4")]
        [HttpGet("GetTournamentsInformation")]
        public async Task<IActionResult> GetTournamentsInformation([FromBody] GetTournamentInformationDto getTournamentInformation)
        {
            if (!Request.Headers.ContainsKey("X-TimeZone"))
            {
                return BadRequest(ApiResponse<IEnumerable<TournamentsInformationDto>>.ErrorResponse("La zona horaria es requerida."));
            }

            var timeZoneId = Request.Headers["X-TimeZone"].ToString();

            // Pasar la zona horaria junto con el DTO al servicio
            var tournaments = await _tournamentService.GetTournamentsInformationAsync(getTournamentInformation, timeZoneId);

            if (tournaments is null || !tournaments.Any())
                return NotFound(ApiResponse<IEnumerable<TournamentsInformationDto>>.ErrorResponse("Torneos no encontrados."));

            try
            {
                return Ok(ApiResponse<IEnumerable<TournamentsInformationDto>>.SuccessResponse("Torneos obtenidos exitosamente.", tournaments));
            }
            catch (InvalidOperationException ex)
            {
                // Registrar el error con detalles más completos
                _logger.LogError($"Error en GetTournamentsInformation: {ex.Message}. StackTrace: {ex.StackTrace}");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error desconocido: {ex.Message}", ex);
                return StatusCode(500, "Ocurrió un error inesperado.");
            }
        }



        [Authorize(Roles = "4")] 
        [HttpGet("ShowCardsFromTournament")]
        public async Task<IActionResult> ShowCardsFromTournament([FromBody] TournamentRequestToResolveDto tournamentId)
        {
            var cards = await _cardService.GetAllCardsFromTournamentAsync(tournamentId);

            if (cards == null || !cards.Any())
                return NotFound(ApiResponse<IEnumerable<ShowCardsDto>>.ErrorResponse("Cartas no encontradas."));

            var response = ApiResponse<IEnumerable<ShowCardsDto>>.SuccessResponse("Cartas obtenidas exitosamente.", cards);
            return Ok(response);
        }

        [Authorize(Roles = "4")]
        [HttpPost("SetTournamentDecks")]
        public async Task<IActionResult> SetTournamentDecks([FromBody] TournamentDecksDto tournamentDeckDto)
        {
            if (tournamentDeckDto == null)
                return BadRequest("Invalid tournament deck data.");

            await _tournamentService.InsertTournamentDecksAsync(tournamentDeckDto);

            var response = ApiResponse<object>.SuccessResponse("Deck agregado exitosamente");
            return Created("", response);
        }

    }
}
