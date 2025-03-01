
using CTDataModels.Users;
using CTDataModels.Users.LogIn;
using CTDto.Users;
using CTDto.Users.Admin;
using CTDto.Users.Judge;
using CTDto.Users.LogIn;
using CTDto.Users.Organizer;
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
        Task<int> CreateWhitHashedPasswordAsync(UserRequestDto LoginDto);
        Task<UserDto> GetUserWhitTokenAsync(int id);
        Task<UserModel> GetUserDataByNameAsync(string fullname);

        Task<LogInResponseModel> NewLogInAsync(string fullname, string passcode);

        //Judges
        Task<IEnumerable<JudgeDto>> GetAllJudgesAsync();
        Task<IEnumerable<CountriesListDto>> GetAllCountriesAsync();


        //Admin
        Task<int> CreateUserAsync(UserCreationDto userDto);
        Task AlterUserAsync(AlterUserDto userDto);

        Task SoftDeleteUserAsync(SoftDeleteUserDto userDto);
    }
}
