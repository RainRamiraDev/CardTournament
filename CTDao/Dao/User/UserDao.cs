using CTDao.Interfaces.User;
using CTDataModels.Users;
using CTDataModels.Users.LogIn;
using CTDataModels.Users.Organizer;
using Dapper;
using DataAccess;
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

        public UserDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserModel> LogInAsync(string fullname)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(QueryLoader.GetQuery("QueryLogin"), new { fullname });
                return user;
            }
        }

        public async Task<UserModel> GetUserWhitTokenAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(QueryLoader.GetQuery("QueryGetUserWhitToken"), new { id });
                return user;
            }
        }

        public async Task<int> CreateWhitHashedPasswordAsync(LoginRequestModel user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var affectedRows = await connection.ExecuteAsync(QueryLoader.GetQuery("QueryFirstLogIn"), new
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
                var judges = await connection.QueryAsync<UserModel>(QueryLoader.GetQuery("QueryGetAllJudges"));
                return judges;
            }
        }

        public async Task<int> GetPlayerKiByIdAsync(int playerId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var ki = await connection.QueryFirstOrDefaultAsync<int>(
                    QueryLoader.GetQuery("QueryGetPlayersRankIds"),
                    new { Id = playerId }
                );

                return ki; 
            }
        }

        public async Task<IEnumerable<CountriesListModel>> GetAllCountriesAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var countries = await connection.QueryAsync<CountriesListModel>(QueryLoader.GetQuery("QueryGetAllCountries"));
                return countries;
            }
        }
    }
}
