using CTDao.Dao.Security;
using CTDao.Interfaces.User;
using CTDataModels.Users;
using CTDataModels.Users.LogIn;
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

        private readonly PasswordHasher _passwordHasher;

        public UserService(IUserDao userDao, PasswordHasher passwordHasher)
        {
            _userDao = userDao;
            _passwordHasher = passwordHasher;
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

        public async Task<int> CreateWhitHashedPasswordAsync(LoginRequestDto LoginDto)
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

    }
}
