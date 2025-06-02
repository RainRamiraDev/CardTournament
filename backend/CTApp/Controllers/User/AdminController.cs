using CTApp.Response;
using CTDto.Tournaments;
using CTDto.Users;
using CTDto.Users.Admin;
using CTDto.Users.LogIn;
using CTService.Interfaces.Tournaments;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;

        public AdminController(IUserService userService, ITournamentService tournamentService)
        {
            _userService = userService;
            _tournamentService = tournamentService;
        }

       // [Authorize(Roles = "2,1,3,4")]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreationDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");


            var createdUserId = await _userService.CreateUserAsync(userDto);
            var user = new UserRequestDto
            {
                Id = createdUserId,
                Fullname = userDto.Fullname,
                Id_Rol = userDto.Id_Rol,
                Passcode = userDto.Passcode,
            };

            var response = ApiResponse<UserRequestDto>.SuccessResponse("Usuario creado exitosamente", user);
            return Created(string.Empty, response);
        }

       // [Authorize(Roles = "2,1,3,4")]
        [HttpPut("AlterUser")]
        public async Task<IActionResult> AlterUser([FromBody] AlterUserDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");

            await _userService.AlterUserAsync(userDto);

            var newUser = new ShowUserDto
            {
                Fullname = userDto.New_Fullname,
                Id_Rol = userDto.New_Id_Rol,
                IdCountry = userDto.New_IdCountry,
                Alias = userDto.New_Alias,
                Email = userDto.New_Email,
                Avatar_Url = userDto.New_Avatar_Url,
            };

            var response = ApiResponse<ShowUserDto>.SuccessResponse("Usuario actualizado exitosamente", newUser);

            return Ok(response);
        }



        [Authorize(Roles = "1")]
        [HttpDelete("DeactivateUser")]
        public async Task<IActionResult> DeactivateUser([FromBody] SoftDeleteUserDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");

            await _userService.SoftDeleteUserAsync(userDto);

            var response = ApiResponse<SoftDeleteUserDto>.SuccessResponse("Usuario dado de baja exitosamente");
            return Created(string.Empty, response);
        }


        //todo: cambiar el dto para tener uno propio
        [Authorize(Roles = "2,1")]
        [HttpDelete("CancelTournament")]
        public async Task<IActionResult> SoftDeleteTournament([FromBody] TournamentRequestToResolveDto tournamentDto)
        {
            if (tournamentDto == null)
                return BadRequest("Invalid user data.");

            await _tournamentService.SoftDeleteTournamentAsync(tournamentDto);

            var response = ApiResponse<SoftDeleteUserDto>.SuccessResponse("Torneo cancelado exitosamente");
            return Created(string.Empty, response);
        }




    }
}
