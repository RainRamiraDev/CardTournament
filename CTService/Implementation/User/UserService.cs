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
using CTService.Interfaces.User;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.User
{
    public class UserService : IUserService
    {

        private readonly IUserDao _userDao;

        private readonly ITournamentDao _tournamentDao;

        private readonly PasswordHasher _passwordHasher;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher, ITournamentDao tournamentDao)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
            _tournamentDao = tournamentDao;
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

        public async Task<UserModel> LogInAsync(string fullname, string passcode)
        {
            var user = await _userDao.LogInAsync(fullname);
            if (user == null)
            {
                return null;
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(passcode, user.Passcode);
            if (!isPasswordValid)
            {
                return null;
            }

            return user;
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
                Fullname = judge.Fullname,
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
                Disqualifications = 0,
                Ki = calculateUserKi,
            };

            await ValidateUserCreation(userModel);
            return await _userDao.CreateUserAsync(userModel);
        }

        public async Task<int> CalculateUserKi(UserCreationDto userDto)
        {
            return userDto.Id_Rol != 4 ? 0 : new Random().Next(1000, 1000001);
        }

        public async Task ValidateUserCreation(UserCreationModel user)
        {

            var registeredCountries = await _tournamentDao.ValidateCountriesFromDbAsync(user.Id_Country);
            if (!registeredCountries.Any())
                throw new ArgumentException("El país especificado no está registrado.");


            bool aliasExists = await _userDao.ValidateUsersAlias(user.Alias);
            if (aliasExists)
                throw new ArgumentException("El Alias especificado ya está registrado y no puede repetirse.");


            var emailsExist = await _userDao.ValidateUserEmail(user.Email);
            if (emailsExist)
                throw new ArgumentException("El Email especificado ya está registrado.");
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
                throw new ArgumentException("El Usuario especificado no está registrado.");

            var registeredCountries = await _tournamentDao.ValidateCountriesFromDbAsync(user.New_IdCountry);
            if (!registeredCountries.Any())
                throw new ArgumentException("El país especificado no está registrado.");

            bool aliasExists = await _userDao.ValidateUsersAlias(user.New_Alias);
            if (aliasExists)
                throw new ArgumentException("El Alias especificado ya está registrado y no puede repetirse.");

            var emailsExists = await _userDao.ValidateUserEmail(user.New_Email);
            if (emailsExists)
                throw new ArgumentException("El Email especificado ya está registrado y no puede repetirse.");
   
            return response;
        }

        public async Task SoftDeleteUserAsync(SoftDeleteUserDto userDto)
        {
              await _userDao.SoftDeleteUserAsync(userDto.Id_User);

        }
    }
}

