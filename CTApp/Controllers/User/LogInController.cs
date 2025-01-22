using CTApp.Response;
using CTConfigurations;
using CTDto.Users;
using CTDto.Users.LogIn;
using CTService.Interfaces.RefreshToken;
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


        // Método auxiliar para gestionar cookies
        private void ManageRefreshTokenCookie(string refreshTokenValue, DateTime? expirationDate = null)
        {
            Response.Cookies.Append("RefreshToken", refreshTokenValue,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expirationDate ?? DateTime.UtcNow.AddDays(_keysConfiguration.ExpirationDays) // Usar el valor desde la configuración
                });
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LoginRequestDto loginRequest)
        {
            var user = await _userService.LogInAsync(loginRequest.Fullname, loginRequest.Passcode);

            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse(
                    new List<string> { "Usuario o contraseña incorrectos." }
                ));
            }

            var accessToken = _refreshTokenService.GenerateAccessToken(user.Id_User, user.Fullname,user.Id_Rol);

            Guid refreshToken = Guid.NewGuid();
            DateTime expirationDate = DateTime.UtcNow.AddDays(_keysConfiguration.ExpirationDays); // Usar el valor desde la configuración

            await _refreshTokenService.SaveRefreshTokenAsync(refreshToken, user.Id_User, expirationDate);

            ManageRefreshTokenCookie(refreshToken.ToString(), expirationDate);  // Utilizamos el método auxiliar para la cookie
            return Ok(new { AccessToken = accessToken });
        }


        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshTokenFromCookie = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshTokenFromCookie) || !Guid.TryParse(refreshTokenFromCookie, out Guid refreshToken))
            {
                return Unauthorized(new { Message = "No se encontró un refresh token válido." });
            }

            var (newAccessToken, newRefreshToken) = await _refreshTokenService.RefreshAccessTokenAsync(refreshToken);

            ManageRefreshTokenCookie(newRefreshToken.ToString(), DateTime.UtcNow.AddDays(7)); // Usamos el método auxiliar para la nueva cookie
            return Ok(new { AccessToken = newAccessToken });
        }



        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            string refreshTokenFromCookies = Request.Cookies["RefreshToken"];

            Guid.TryParse(refreshTokenFromCookies, out Guid refreshToken);

            bool result = await _refreshTokenService.LogoutAsync(refreshToken);

            if (!result)
            {
                return NotFound(new { Message = "El token no fue encontrado o ya se eliminó." });
            }

            ManageRefreshTokenCookie("", DateTime.Now);
            return Ok(new { Message = "Sesión cerrada exitosamente." });
        }


        [HttpPost("FirstLogIn")]
        public async Task<IActionResult> CreateUserWhitHashedPassword([FromBody] LoginRequestDto loginDto)
        {
            var userId = await _userService.CreateWhitHashedPasswordAsync(loginDto);
            var user = new UserDto
            {
                Fullname = loginDto.Fullname,
                Passcode = loginDto.Passcode,
                Id_Rol = loginDto.Id_Rol  // Aseguramos que el id_rol también esté en la respuesta
            };

            var response = ApiResponse<LoginRequestDto>.SuccessResponse("Usuario creado exitosamente", loginDto);
            return Created(string.Empty, response);
        }

    }
}
