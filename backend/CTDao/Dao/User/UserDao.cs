using CTDao.Interfaces.User;
using CTDataModels.Users;
using CTDataModels.Users.Admin;
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

        public async Task<UserModel> GetUserDataByNameAsync(string fullname)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(QueryLoader.GetQuery("QueryGetUserDataByName"), new { fullname });
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

        public async Task<int> CreateUserAsync(UserCreationModel user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var newUserId = await connection.ExecuteScalarAsync<int>(QueryLoader.GetQuery("QueryCreateUser"), new
                {
                    user.Id_Country,
                    user.Id_Rol,
                    user.Fullname,
                    user.Passcode,
                    user.Alias,
                    user.Email,
                    user.Avatar_Url,
                    user.Games_Won,
                    user.Games_Lost,
                    user.Ki,
                });

                return newUserId;
            }
        }

        public async Task<bool> ValidateUserEmail(string userEmail)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int emailCount = await connection.ExecuteScalarAsync<int>(
                    QueryLoader.GetQuery("QueryVerifyEmail"),
                    new { email = userEmail }
                );
                return emailCount > 0;
            }
        }

        public async Task<bool> ValidateUsersAlias(string userAlias)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                int aliasCount = await connection.ExecuteScalarAsync<int>(
                    QueryLoader.GetQuery("QueryVerifyAlias"),
                    new { alias = userAlias }
                );
                return aliasCount > 0;
            }
        }

        public async Task<UserModel> GetUserById(int id_user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(
                    QueryLoader.GetQuery("QueryGetUserById"),
                    new { Id_user = id_user }
                );
                return user;
            }
        }

        public async Task AlterUserAsync(AlterUserModel user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var affectedRows = await connection.ExecuteAsync(QueryLoader.GetQuery("QueryAlterUser"), new
                {
                   id_country =  user.New_IdCountry,
                   id_rol = user.New_Id_Rol,
                   fullname = user.New_Fullname,
                   alias =  user.New_Alias,
                   email =  user.New_Email,
                   avatar_url = user.New_Avatar_Url,
                   id_user = user.Id_User,
                });
            }
        }

        public async Task SoftDeleteUserAsync(int id_user)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var affectedRows = await connection.ExecuteAsync(QueryLoader.GetQuery("QuerySoftDeleteUser"), new
                {
                    id_user = id_user
                });
            }
        }
    }
}
