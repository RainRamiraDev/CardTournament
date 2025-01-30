
using CTDataModels.Users;
using CTDto.Users;
using CTDto.Users.Judge;
using CTDto.Users.LogIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Interfaces.User
{
    public interface IUserService
    {
        //LogIn
        Task<int> CreateWhitHashedPasswordAsync(LoginRequestDto userDto);
        Task<UserDto> GetUserWhitTokenAsync(int id);
        Task<UserModel> LogInAsync(string fullname, string passcode);

        //Judges
        Task<IEnumerable<JudgeDto>> GetAllJudgesAsync();
    }
}
