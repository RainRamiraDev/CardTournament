using CTDao.Interfaces.User;
using CTDataModels.Users;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDao.Dao.User
{
    public class UserDao : IUserDao
    {
        private readonly string _connectionString;

        private readonly string QueryGetUserWhitToken = "SELECT * FROM T_Users WHERE Id_User = @Id_user";
        private readonly string QueryFirstLogIn = "INSERT INTO T_Users (Fullname,Passcode) VALUES (@Fullname,@Passcode)";
        private readonly string QueryLogin = "SELECT * FROM T_Users WHERE Fullname = @Fullname";

        public UserDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserModel> LogInAsync(string fullname)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(QueryLogin, new { fullname });
                return user;
            }
        }

        public async Task<UserModel> GetUserWhitTokenAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(QueryGetUserWhitToken, new { id });
                return user;
            }
        }

        public async Task<int> CreateWhitHashedPasswordAsync(UserModel user)
        {

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var affectedRows = await connection.ExecuteAsync(QueryFirstLogIn, new
                {
                    user.Fullname,
                    user.Passcode
                });
                return affectedRows;
            }
        }

    }
}
