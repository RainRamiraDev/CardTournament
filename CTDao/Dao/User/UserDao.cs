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

        private readonly string QueryGetUserWhitToken = "SELECT * FROM T_Users WHERE Id_User = @IdUser";
        //private readonly string QueryInsert = "INSERT INTO T_Users (Fullname, Email, Passcode) VALUES (@Nombre, @Email, @Passcode)";
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

    }
}
