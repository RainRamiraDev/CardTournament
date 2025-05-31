using CTConfigurations;
using CTDao.Dao.Security;
using CTDao.Dao.User;
using CTDao.Interfaces.Tournaments;
using CTDao.Interfaces.User;
using CTDataModels.Users;
using CTDataModels.Users.Admin;
using CTDataModels.Users.LogIn;
using CTDto.Card;
using CTDto.Users;
using CTDto.Users.Admin;
using CTDto.Users.Judge;
using CTDto.Users.LogIn;
using CTDto.Users.Organizer;
using CTService.Implementation.RefreshToken;
using CTService.Interfaces.RefreshToken;
using CTService.Interfaces.User;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.User
{
    public class UserService : IUserService
    {

        private readonly IUserDao _userDao;

        private readonly ITournamentDao _tournamentDao;

        private readonly PasswordHasher _passwordHasher;

        private readonly IRefreshTokenService _refreshTokenService;

        private readonly KeysConfiguration _keysConfiguration;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher, ITournamentDao tournamentDao, IRefreshTokenService refreshTokenService, IOptions<KeysConfiguration> keysConfiguration, IHttpContextAccessor httpContextAccessor)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _tournamentDao = tournamentDao;
            _refreshTokenService = refreshTokenService; 
            _keysConfiguration = keysConfiguration.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto> GetUserWhitTokenAsync(int id)
        {
            var userModel = await _userDao.GetUserWhitTokenAsync(id);
            if (userModel == null) return null;

            return new UserDto
            {
                Id_User = userModel.Id_User,
                Fullname = userModel.Fullname,
                Email = userModel.Email
            };
        }


        public async Task<UserModel> GetUserDataByNameAsync(string fullname)
        {
            var user = await _userDao.GetUserDataByNameAsync(fullname);

            if (user == null)
                throw new KeyNotFoundException("El usuario especificado con ese nombre no se encuentra registrado");

            return user;
        }

        public async Task<LogInResponseModel> NewLogInAsync(string fullname, string passcode)
        {

            var user = await GetUserDataByNameAsync(fullname);

            bool isPasswordValid = _passwordHasher.VerifyPassword(passcode, user.Passcode);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Contraseña incorrecta");

            var accessToken = await _refreshTokenService.GenerateAccessTokenAsync(user.Id_User, user.Fullname, user.Id_Rol);

            Guid refreshToken = Guid.NewGuid();
            DateTime expirationDate = DateTime.UtcNow.AddDays(_keysConfiguration.ExpirationDays);

            await _refreshTokenService.SaveRefreshTokenAsync(refreshToken, user.Id_User, expirationDate);


            var loginresponse = new LogInResponseModel
            {
                RefreshToken = refreshToken,
                ExpirationDate = expirationDate,
                AccessToken = accessToken
            };

            return loginresponse;

        }

        public async Task<int> CreateWhitHashedPasswordAsync(UserRequestDto LoginDto)
        {
            string hashedPassword = _passwordHasher.HashPassword(LoginDto.Passcode);

            var loginModel = new LoginRequestModel
            {
                Fullname = LoginDto.Fullname,
                Passcode = hashedPassword,
                Id_Rol = LoginDto.Id_Rol 
            };

            return await _userDao.CreateWhitHashedPasswordAsync(loginModel);
        }

        public async Task<IEnumerable<JudgeDto>> GetAllJudgesAsync()
        {
            var userModels = await _userDao.GetAllJudgeAsync();
            return userModels.Select(judge => new JudgeDto
            {
                Alias = judge.Alias,
                Email = judge.Email,
                Avatar_Url = judge.Avatar_Url,
                Country = judge.Country,
            });
        }

        public async Task<IEnumerable<CountriesListDto>> GetAllCountriesAsync()
        {
            var countryModels = await _userDao.GetAllCountriesAsync();

            return countryModels.Select(country => new CountriesListDto
            {
                Id_country = country.Id_country,
                Country_name = country.Country_name
            }).ToList();
        }

        public async Task<int>CreateUserAsync(UserCreationDto userDto)
        {

            string hashedPassword = _passwordHasher.HashPassword(userDto.Passcode);

            var calculateUserKi = await CalculateUserKi(userDto);

            var userModel = new UserCreationModel
            {
                Id_Country = userDto.Id_Country,
                Id_Rol = userDto.Id_Rol,
                Fullname = userDto.Fullname,
                Passcode = hashedPassword,
                Alias = userDto.Alias,
                Email = userDto.Email,
                Avatar_Url = userDto.Avatar_Url,
                Games_Won = 0,
                Games_Lost = 0,
                Ki = calculateUserKi,
            };

            await ValidateUserCreation(userModel);
        //    await ValidateUserRol(userModel.Id_Rol);
            return await _userDao.CreateUserAsync(userModel);
        }

        public async Task ValidateUserRol(int rolToCreat)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            var userRolClaim = userClaims?.FindFirst("UserRole");

            if (userRolClaim == null)
                throw new InvalidOperationException("Error al recuperar el rol");

            if (!int.TryParse(userRolClaim.Value, out int userRol))
                throw new InvalidOperationException("El Rol del usuario en el token no es válido.");

            if (userRol == 2) 
                if (rolToCreat != 3 && rolToCreat != 4)
                    throw new UnauthorizedAccessException("Un organizador solo puede crear jueces o jugadores.");

            else if (userRol == 4) 
                if (rolToCreat != 4)
                    throw new UnauthorizedAccessException("Un jugador solo puede crear otros jugadores.");

            else if (userRol != 1) 
                throw new UnauthorizedAccessException("No tienes permisos para crear usuarios.");
        }

        public async Task<int> CalculateUserKi(UserCreationDto userDto)
        {
            return userDto.Id_Rol != 4 ? 0 : new Random().Next(1000, 1000001);
        }

        public async Task ValidateUserCreation(UserCreationModel user)
        {

            var registeredCountries = await _tournamentDao.ValidateCountriesFromDbAsync(user.Id_Country);
            if (!registeredCountries.Any())
                throw new KeyNotFoundException("El país especificado no está registrado.");


            bool aliasExists = await _userDao.ValidateUsersAlias(user.Alias);
            if (aliasExists)
                throw new InvalidOperationException("El Alias especificado ya está registrado y no puede repetirse.");


            var emailsExist = await _userDao.ValidateUserEmail(user.Email);
            if (emailsExist)
                throw new InvalidOperationException("El Email especificado ya está registrado.");
        }

        public async Task AlterUserAsync(AlterUserDto userDto)
        {
            var alterUserModel = new AlterUserModel
            {
              Id_User = userDto.Id_User,
              New_IdCountry = userDto.New_IdCountry,
              New_Alias = userDto.New_Alias,
              New_Avatar_Url = userDto.New_Avatar_Url,
              New_Email = userDto.New_Email,
              New_Fullname = userDto.New_Fullname,
              New_Id_Rol  = userDto.New_Id_Rol,
           };

            var isValidUser = await ValidateUserModificationAsync(alterUserModel);
            if (isValidUser)
                await _userDao.AlterUserAsync(alterUserModel);
        }

        public async Task<bool> ValidateUserModificationAsync(AlterUserModel user)
        {
            var response = true;

            UserModel oldUser = await _userDao.GetUserById(user.Id_User);
            if (oldUser == null)
                throw new KeyNotFoundException("El Usuario especificado no está registrado.");

            var registeredCountries = await _tournamentDao.ValidateCountriesFromDbAsync(user.New_IdCountry);
            if (!registeredCountries.Any())
                throw new KeyNotFoundException("El país especificado no está registrado.");

            bool aliasExists = await _userDao.ValidateUsersAlias(user.New_Alias);
            if (aliasExists)
                throw new InvalidOperationException("El Alias especificado ya está registrado y no puede repetirse.");

            var emailsExists = await _userDao.ValidateUserEmail(user.New_Email);
            if (emailsExists)
                throw new InvalidOperationException("El Email especificado ya está registrado y no puede repetirse.");

            await ValidateUserRol(oldUser.Id_Rol);

            return response;
        }

        public async Task SoftDeleteUserAsync(SoftDeleteUserDto userDto)
        {
            var user = await _userDao.GetUserById(userDto.Id_User);
            if (user.Available == 0)
                throw new InvalidOperationException("El usuario especificado ya ha sido eliminado");
           
            await ValidateUserRol(user.Id_Rol);
            await _userDao.SoftDeleteUserAsync(userDto.Id_User);
        }
    }
}

