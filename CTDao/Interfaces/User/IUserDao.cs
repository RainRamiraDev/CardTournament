using CTDataModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Interfaces.User
{
    public interface IUserDao
    {
        //log In

        Task<int> CreateWhitHashedPasswordAsync(UserModel user);
        Task<UserModel> GetUserWhitTokenAsync(int id);
        Task<UserModel> LogInAsync(string fullname);

        //Judge
        Task<IEnumerable<UserModel>> GetAllJudgeAsync();


    }
}
