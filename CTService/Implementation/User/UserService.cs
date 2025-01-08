using CTDao.Dao.Security;
using CTDao.Interfaces.User;
using CTDataModels.Users;
using CTDto.Users;
using CTService.Interfaces.User;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
