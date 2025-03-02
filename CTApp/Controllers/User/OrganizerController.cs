﻿using CTApp.Response;
using CTDataModels.Tournamets;
using CTDto.Card;
using CTDto.Tournaments;
using CTDto.Users;
using CTDto.Users.Judge;
using CTDto.Users.Organizer;
using CTService.Implementation.User;
using CTService.Interfaces.Card;
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
        private readonly ICardService _cardService;

        public OrganizerController(IUserService userService, ITournamentService tournamentService, ICardService cardService)
        {
            _userService = userService;
            _tournamentService = tournamentService;
            _cardService = cardService;
        }


        [Authorize(Roles = "1")]
        [HttpGet("GetJudges")]
        public async Task<IActionResult> GetJudges()
        {
            var judges = await _userService.GetAllJudgesAsync();

            if (judges is null || !judges.Any())
                return NotFound(ApiResponse<IEnumerable<JudgeDto>>.ErrorResponse("Jueces no encontrados."));

            return Ok(ApiResponse<IEnumerable<JudgeDto>>.SuccessResponse("Jueces obtenidos exitosamente.", judges));
        }


        [Authorize(Roles = "1")]
        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _userService.GetAllCountriesAsync();
            return Ok(ApiResponse<IEnumerable<CountriesListDto>>.SuccessResponse("Paises obtenidos exitosamente.", countries));
        }


        [Authorize(Roles = "1")]
        [HttpGet("GetSeries")]
        public async Task<IActionResult> GetSeries()
        {
            var series = await _cardService.GetAllSeriesAsync();
            return Ok(ApiResponse<IEnumerable<SeriesListDto>>.SuccessResponse("Series obtenidos exitosamente.", series));
        }



        [Authorize(Roles = "1")]
        [HttpPost("CreateTournament")]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentDto tournamentDto)
        {
            if (tournamentDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Los datos del torneo son inválidos."));

            var id = await _tournamentService.CreateTournamentAsync(tournamentDto);

            if (id == 0)
                throw new InvalidOperationException("Error al crear el torneo.");

            return Created("", ApiResponse<object>.SuccessResponse("Torneo creado exitosamente.", new { id }));
        }





        //TODO: COMPLETAR EL ALTER TOURNAMENT
        [Authorize(Roles = "1")]
        [HttpPost("AlterTournament")]
        public async Task<IActionResult> AlterTournament([FromBody] AlterTournamentDto tournamentDto)
        {
            if (tournamentDto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Los datos del torneo son inválidos."));

            await _tournamentService.AlterTournamentAsync(tournamentDto);

            return Created("", ApiResponse<object>.SuccessResponse("Torneo modificado exitosamente."));
        }

        //TODO: COMPLETAR EL CalcelTournament
        //[Authorize(Roles = "1")]
        //[HttpPost("CalcelTournament")]
        //public async Task<IActionResult> CalcelTournament([FromBody] CalcelTournamentDto tournamentDto)
        //{
        //    if (tournamentDto == null)
        //        return BadRequest(ApiResponse<object>.ErrorResponse("Los datos del torneo son inválidos."));

        //    var id = await _tournamentService.SoftDeletelTournamentAsync(tournamentDto);

        //    if (id == 0)
        //        throw new InvalidOperationException("Error al dar de baja  el torneo.");

        //    return Created("", ApiResponse<object>.SuccessResponse("Torneo dado de baja exitosamente.", new { id }));
        //}




    }
}
