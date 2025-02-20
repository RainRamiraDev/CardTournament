using CTDao.Dao.Security;
using CTDao.Dao.User;
using CTDao.Interfaces.Tournaments;
using CTDao.Interfaces.User;
using CTDataModels.Users;
using CTDataModels.Users.LogIn;
using CTDto.Card;
using CTDto.Users;
using CTDto.Users.Judge;
using CTDto.Users.LogIn;
using CTDto.Users.Organizer;
using CTService.Interfaces.User;
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

        public async Task<int> CreateWhitHashedPasswordAsync(FirstLogInDto LoginDto)
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

        public async Task<bool> ValidateIfOrganizerAsync(UserModel user)
        {
            bool response = false;
          var organizers = await _tournamentDao.GetUsersFromDbAsync(1);
            if(organizers.Contains(user.Id_User) || user.Id_Rol == 1)
               response = true;
            return response;
        }


        public async Task CreateUserAsync(UserCreationDto userDto)
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

            var isValidUser = await ValidateUserCreation(userModel);

            if (isValidUser)
                await _userDao.CreateUserAsync(userModel);
        }

        public async Task<int> CalculateUserKi(UserCreationDto userDto)
        {
            return userDto.Id_Rol != 4 ? 0 : new Random().Next(1000, 1000001);
        }


        public async Task<bool> ValidateUserCreation(UserCreationModel user)
        {
            var response = true;

            var registeredCountries = await _tournamentDao.GetCountriesFromDbAsync();
            if (!registeredCountries.Contains(user.Id_Country))
                throw new ArgumentException("El país especificado no está registrado.");

            var registeredAlias = await _userDao.GetAllUsersAlias();
            if (registeredAlias.Contains(user.Alias))
                throw new ArgumentException("El Alias especificado ya está registrado.");

            var registeredEmails = await _userDao.GetAllUsersEmails();
            if (registeredEmails.Contains(user.Email))
                throw new ArgumentException("El Email especificado ya está registrado.");

            return response;
        }


    }
}
