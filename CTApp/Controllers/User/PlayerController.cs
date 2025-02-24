﻿using CTApp.Response;
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
        [HttpGet("GetTournamentsInformation")]
        public async Task<IActionResult> GetTournamentsInformation([FromBody] GetTournamentInformationDto getTournamentInformation )
        {
            var tournaments = await _tournamentService.GetTournamentsInformationAsync(getTournamentInformation);

            if (tournaments is null || !tournaments.Any())
                return NotFound(ApiResponse<IEnumerable<TournamentsInformationDto>>.ErrorResponse("Torneos no encontrados."));

            return Ok(ApiResponse<IEnumerable<TournamentsInformationDto>>.SuccessResponse("Torneos obtenidos exitosamente.", tournaments));
        }


        [Authorize(Roles = "4|1|2|3")] 
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
        public async Task<IActionResult> SetTournamentDecks([FromBody] TournamentDecksDto tournamentDeckDto)
        {
            try
            {
                await _tournamentService.InsertTournamentDecksAsync(tournamentDeckDto); //cambiar
                return Created("", new { message = "Deck agregado Exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

    }



}
