using CTDao.Interfaces.User;
using CTDataModels.Users;
using CTDataModels.Users.LogIn;
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
        private readonly string QueryFirstLogIn = "INSERT INTO T_Users (Fullname, Passcode, Id_Rol) VALUES (@Fullname, @Passcode, @Id_Rol)";
        private readonly string QueryLogin = "SELECT Id_User, Fullname, Passcode, Id_Rol FROM T_Users WHERE Fullname = @Fullname";
        private readonly string QueryGetAllJudges = "SELECT u.Fullname,u.Alias,u.Email,c.country_name as Country,u.avatar_url FROM t_users u JOIN t_countries c ON u.id_country = c.id_country WHERE Id_rol = 3";

        private readonly string QueryGetPlayersRankIds = @"SELECT rank FROM t_users WHERE Id_user IN @Ids;";

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

        public async Task<int> CreateWhitHashedPasswordAsync(LoginRequestModel user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var affectedRows = await connection.ExecuteAsync(QueryFirstLogIn, new
                {
                    user.Fullname,
                    user.Passcode,
                    user.Id_Rol
                });
                return affectedRows;
            }
        }

        public async Task<IEnumerable<UserModel>> GetAllJudgeAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var judges = await connection.QueryAsync<UserModel>(QueryGetAllJudges);
                return judges;
            }
        }

        public async Task<List<UserModel>> GetPlayersRanksByIdAsync(List<int> playersIds)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Usamos un parámetro de lista en Dapper para pasarlo a la consulta
                var playerRanks = await connection.QueryAsync<UserModel>(QueryGetPlayersRankIds, new { Ids = playersIds });

                return playerRanks.ToList();
            }
        }
    }
}
