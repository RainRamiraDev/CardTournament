
using CardApp.Interfaces;
using CTService.Interfaces.User;
using DataAccessApp.Dtos.Users.Admin;
using DataAccessApp.Dtos.Users.Judge;
using DataAccessApp.Dtos.Users.Organizer;
using DataAccessApp.Dtos.Users.Player;
using GameApp.Dtos.Card;
using GameApp.Response;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.Controllers.User
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {


        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //[HttpGet("GetAllPlayers")]
        //public async Task<IActionResult> GetAllPlayers()
        //{
        //    try
        //    {
        //        var players = await _userService.GetAllPlayersAsync();

        //        if (players == null || !players.Any())
        //        {
        //            return NotFound(ApiResponse<IEnumerable<PlayerDto>>.ErrorResponse("Jugadores no encontrados."));
        //        }

        //        var response = ApiResponse<IEnumerable<PlayerDto>>.SuccessResponse("Jugadores obtenidos exitosamente.", players);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errors = new List<string> { "Ocurrió un error al obtener los jugadores." };
        //        var stackTrace = ex.StackTrace;
        //        var response = ApiResponse<PlayerDto>.ErrorResponse(errors, stackTrace);
        //        return BadRequest(response);
        //    }
        //}


        //[HttpGet("GetAllJudges")]
        //public async Task<IActionResult> GetAllJudges()
        //{
        //    try
        //    {
        //        var judges = await _userService.GetAllJudgesAsync();

        //        if (judges == null || !judges.Any())
        //        {
        //            return NotFound(ApiResponse<IEnumerable<JudgeDto>>.ErrorResponse("Juezes no encontrados."));
        //        }

        //        var response = ApiResponse<IEnumerable<JudgeDto>>.SuccessResponse("Juezes obtenidos exitosamente.", judges);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errors = new List<string> { "Ocurrió un error al obtener los juezes." };
        //        var stackTrace = ex.StackTrace;
        //        var response = ApiResponse<JudgeDto>.ErrorResponse(errors, stackTrace);
        //        return BadRequest(response);
        //    }
        //}


        //[HttpGet("GetAllOrganizers")]
        //public async Task<IActionResult> GetAllOrganizers()
        //{
        //    try
        //    {
        //        var organizers = await _userService.GetAllOrganizersAsync();

        //        if (organizers == null || !organizers.Any())
        //        {
        //            return NotFound(ApiResponse<IEnumerable<OrganizerDto>>.ErrorResponse("Organizadores no encontrados."));
        //        }

        //        var response = ApiResponse<IEnumerable<OrganizerDto>>.SuccessResponse("Organizadores obtenidos exitosamente.", organizers);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errors = new List<string> { "Ocurrió un error al obtener los organizadores." };
        //        var stackTrace = ex.StackTrace;
        //        var response = ApiResponse<OrganizerDto>.ErrorResponse(errors, stackTrace);
        //        return BadRequest(response);
        //    }
        //}

        //[HttpGet("GetAllAdmins")]
        //public async Task<IActionResult> GetAllAdmins()
        //{
        //    try
        //    {
        //        var admins = await _userService.GetAllAdminsAsync();

        //        if (admins == null || !admins.Any())
        //        {
        //            return NotFound(ApiResponse<IEnumerable<AdminDto>>.ErrorResponse("Administradores no encontrados."));
        //        }

        //        var response = ApiResponse<IEnumerable<AdminDto>>.SuccessResponse("Administradores obtenidos exitosamente.", admins);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errors = new List<string> { "Ocurrió un error al obtener los administradores." };
        //        var stackTrace = ex.StackTrace;
        //        var response = ApiResponse<AdminDto>.ErrorResponse(errors, stackTrace);
        //        return BadRequest(response);
        //    }
        //}

    }
}
