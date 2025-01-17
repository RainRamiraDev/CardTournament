

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


        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Fullname) || string.IsNullOrWhiteSpace(loginRequest.Passcode))
                {
                    return BadRequest(ApiResponse<UserDto>.ErrorResponse(
                        new List<string> { "Nombre de usuario y contraseña son requeridos." },
                        "LogIn: Datos inválidos en la solicitud."
                    ));
                }

                var user = await _userService.LogInAsync(loginRequest.Fullname, loginRequest.Passcode);

                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResponse(
                        new List<string> { "Usuario no encontrado o contraseña incorrecta." },
                        $"LogIn: No se encontró el usuario con Nombre: {loginRequest.Fullname}"
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

        //[HttpPost("Logout")]
        //public async Task<IActionResult> Logout([FromBody] Guid refreshToken)
        //{
        //    try
        //    {
        //        bool result = await _refreshTokenService.LogoutAsync(refreshToken);

        //        if (!result)
        //            return NotFound(new { Message = "El token no fue encontrado o ya se eliminó." });

        //        Response.Cookies.Append("RefreshToken", "", new CookieOptions
        //        {
        //            Expires = DateTime.Now,
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict
        //        });

        //        return Ok(new { Message = "Sesión cerrada exitosamente." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = "Ocurrió un error al intentar cerrar sesión.", Details = ex.Message });
        //    }
        //}

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutDto request)
        {
            if (request.RefreshToken == Guid.Empty)
                return BadRequest(new { Message = "El refresh token es inválido." });

            bool result = await _refreshTokenService.LogoutAsync(request.RefreshToken);

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


        [HttpPost("FirstLogIn")]
        public async Task<IActionResult> CreateUserWhitHashedPassword([FromBody] UserDto userDto)
        {
            try
            {
                var userId = await _userService.CreateWhitHashedPasswordAsync(userDto);
                var user = new UserDto
                {
                    Fullname = userDto.Fullname,
                    Passcode = userDto.Passcode,
                };
                var response = ApiResponse<UserDto>.SuccessResponse
                    (
                        "Usuario creado exitosamente",
                        userDto
                    );

                return Created(string.Empty, response);

            }
            catch (Exception ex)
            {
                var errors = new List<string> { ex.Message };
                var stackTrace = ex.StackTrace;
                var response = ApiResponse<UserDto>.ErrorResponse(errors, stackTrace);
                return BadRequest(response);
            }
        }





    }
}
