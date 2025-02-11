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
        public async Task<IActionResult> GetAllCards([FromBody] TournamentRequestToResolveDto tournamentId)
        {
            try
            {

                var cards = await _cardService.GetAllCardsAsync(tournamentId);

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


        [Authorize(Roles = "4")]
        [HttpPost("SetTournamentDecks")]
        public async Task<IActionResult> SetTournamentSeries([FromBody] TournamentDecksDto tournamentDeckDto)
        {
            try
            {
                var affectedRows = await _tournamentService.InsertTournamentDecksAsync(tournamentDeckDto);

                if (affectedRows == 0)
                {
                    return StatusCode(500, "Error assigning Decks to tournament.");
                }

                return Created("", new { affectedRows });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message }); // El mensaje de ilustraciones duplicadas se enviará aquí
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

    }



}
