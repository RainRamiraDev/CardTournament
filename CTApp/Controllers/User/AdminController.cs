

using CTApp.Response;
using CTConfigurations;
using CTDto.Users;
using CTService.Interfaces.RefreshToken;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Mvc;

namespace CTApp.Controllers.User
{

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly IUserService _userService;

        private readonly IRefreshTokenService _refreshTokenService;

        private readonly KeysConfiguration _keysConfiguration;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("LogIn/{fullname}/{passcode}")]
        public async Task<IActionResult> LogIn(string fullname, string passcode)
        {
            try
            {
                Console.WriteLine(fullname + "-"+passcode);


                var user = await _userService.LogInAsync(fullname, passcode);

                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResponse(
                        new List<string> { "Usuario no encontrado o contraseña incorrecta." },
                        $"LogIn: No se pudo encontrar al usuario con Nombre: {fullname}"
                    ));
                }

                var accessToken = _refreshTokenService.GenerateAccessToken(user.Id_User, user.Fullname, user.Email);

                Guid refreshToken = Guid.NewGuid();
                DateTime expirationDate = DateTime.UtcNow.AddDays(7);

                await _refreshTokenService.SaveRefreshTokenAsync(refreshToken, user.Id_User, expirationDate);

                Response.Cookies.Append("RefreshToken", refreshToken.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = expirationDate
                    });

                return Ok(new { AccessToken = accessToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(
                    new List<string> { "Ocurrió un error al intentar iniciar sesión." },
                    ex.StackTrace
                ));
            }
        }



        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] Guid oldRefreshToken)
        {
            try
            {
                var refreshTokenFromCookie = Request.Cookies["RefreshToken"];

                if (string.IsNullOrEmpty(refreshTokenFromCookie))
                {
                    return Unauthorized(new { Message = "No se encontró el refresh token." });
                }

                var (newAccessToken, newRefreshToken) = await _refreshTokenService.RefreshAccessTokenAsync(Guid.Parse(refreshTokenFromCookie));

                Response.Cookies.Append("RefreshToken", newRefreshToken.ToString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(7)
                    });

                return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Ocurrió un error al intentar renovar el token.", Details = ex.Message });
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] Guid refreshToken)
        {
            try
            {
                bool result = await _refreshTokenService.LogoutAsync(refreshToken);

                if (!result)
                    return NotFound(new { Message = "El token no fue encontrado o ya se eliminó." });

                Response.Cookies.Append("RefreshToken", "", new CookieOptions
                {
                    Expires = DateTime.Now,
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { Message = "Sesión cerrada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Ocurrió un error al intentar cerrar sesión.", Details = ex.Message });
            }
        }

    }
}
