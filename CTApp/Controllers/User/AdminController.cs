using CTApp.Response;
using CTDto.Tournaments;
using CTDto.Users;
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
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
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
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
          
        }



    }
}
