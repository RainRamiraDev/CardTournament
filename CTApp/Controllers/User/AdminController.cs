using CTDto.Tournaments;
using CTDto.Users;
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
                return Created(string.Empty, new { message = "Usuario creado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
          
        }



    }
}
