using CTApp.Response;
using CTDto.Users.Judge;
using CTService.Implementation.User;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.User
{


    [ApiController]
    [Route("api/[controller]")]

    public class OrganizerController : ControllerBase
    {
        private readonly IUserService _userService;

        public OrganizerController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetJudges")]
        public async Task<IActionResult> GetJudges()
        {
            var judges = await _userService.GetAllJudgesAsync();

            if (judges is null || !judges.Any())
                return NotFound(ApiResponse<IEnumerable<JudgeDto>>.ErrorResponse("Jueces no encontrados."));

            return Ok(ApiResponse<IEnumerable<JudgeDto>>.SuccessResponse("Jueces obtenidos exitosamente.", judges));
        }

    }
}
