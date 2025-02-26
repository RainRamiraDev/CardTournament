using CTApp.Response;
using CTDto.Tournaments;
using CTDto.Users;
using CTDto.Users.Admin;
using CTDto.Users.LogIn;
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

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "2")]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreationDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");
            
                await _userService.CreateUserAsync(userDto);

            var user = new FirstLogInDto
            {
                    
                Fullname = userDto.Fullname,
                Id_Rol = userDto.Id_Rol,
                Passcode = userDto.Passcode,
            };

            var response = ApiResponse<FirstLogInDto>.SuccessResponse("Usuario creado exitosamente", user);
            return Created(string.Empty, response);
        }

        [Authorize(Roles = "2")]
        [HttpPut("AlterUser")]
        public async Task<IActionResult> AlterUser([FromBody] AlterUserDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");

            await _userService.AlterUserAsync(userDto);

            var newUser = new AlterUserDto
            {
                New_Fullname = userDto.New_Fullname,
                New_Id_Rol = userDto.New_Id_Rol,
                New_IdCountry = userDto.New_IdCountry,
                New_Alias = userDto.New_Alias,
                New_Email = userDto.New_Email,     
                New_Avatar_Url = userDto.New_Avatar_Url,
            };

            var response = ApiResponse<AlterUserDto>.SuccessResponse("Usuario creado exitosamente", newUser);
            return Created(string.Empty, response);
        }
    }
}
