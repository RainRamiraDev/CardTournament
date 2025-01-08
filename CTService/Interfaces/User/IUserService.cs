
using CTDataModels.Users;
using CTDto.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Interfaces.User
{
    public interface IUserService
    {
        //Task<int> CreateWhitHashedPasswordAsync(UserDbAlterDto userDto);
        Task<UserDto> GetUserWhitTokenAsync(int id);
        Task<UserModel> LogInAsync(string fullname, string passcode);
    }
}
