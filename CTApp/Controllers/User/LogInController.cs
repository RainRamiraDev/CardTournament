using CTApp.Response;
using CTConfigurations;
using CTDto.Users;
using CTDto.Users.LogIn;
using CTService.Interfaces.RefreshToken;
using CTService.Interfaces.Tournaments;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CTApp.Controllers.User
{

    [ApiController]
    [Route("api/[controller]")]
    public class LogInController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly KeysConfiguration _keysConfiguration;


        public LogInController(IUserService userService, IOptions<KeysConfiguration> keys, IRefreshTokenService refreshToken)
        {
            _userService = userService;
            _refreshTokenService = refreshToken;
            _keysConfiguration = keys.Value;
        }


        private void ManageRefreshTokenCookie(string refreshTokenValue, DateTime? expirationDate = null)
        {
            Response.Cookies.Append("RefreshToken", refreshTokenValue,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expirationDate ?? DateTime.UtcNow.AddDays(_keysConfiguration.ExpirationDays)
                });
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LoginRequestDto loginRequest)
        {

            var userResponse = await _userService.NewLogInAsync(loginRequest.Fullname, loginRequest.Passcode);

            ManageRefreshTokenCookie(userResponse.RefreshToken.ToString(), userResponse.ExpirationDate);

            var response = ApiResponse<string>.SuccessResponse("Inicio de sesión exitoso.", userResponse.AccessToken);

            return Ok(response);
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshTokenFromCookie = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshTokenFromCookie) || !Guid.TryParse(refreshTokenFromCookie, out Guid refreshToken))
                return Unauthorized(new { Message = "No se encontró un refresh token válido." });

            var (newAccessToken, newRefreshToken) = await _refreshTokenService.RefreshAccessTokenAsync(refreshToken);

            ManageRefreshTokenCookie(newRefreshToken.ToString(), DateTime.UtcNow.AddDays(7));

            var response = ApiResponse<string>.SuccessResponse("Inicio de sesión exitoso.", newAccessToken);

            return Ok(response);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            string refreshTokenFromCookies = Request.Cookies["RefreshToken"];

            Guid.TryParse(refreshTokenFromCookies, out Guid refreshToken);

            bool result = await _refreshTokenService.LogoutAsync(refreshToken);

            if (!result)
                return NotFound(new { Message = "El token no fue encontrado o ya se eliminó." });

            ManageRefreshTokenCookie("", DateTime.Now);

            var response = ApiResponse<string>.SuccessResponse("Sesión cerrada exitosamente.");

            return Ok(response);
        }
    }
}
