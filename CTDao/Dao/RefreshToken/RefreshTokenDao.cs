using Dapper;
using MySql.Data.MySqlClient;
using RefreshTokenApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDaoLib.Daos.Interfaces;
namespace UserDaoLib.Daos
{
    public class RefreshTokenDao : IRefreshTokenDao
    {
        private readonly string _connectionString;

        private const string QueryVerifyToken = @"
            SELECT 1
            FROM refresh_tokens 
            WHERE token = @Token AND expiry_date > @Now 
            LIMIT 1";

        private const string QueryDeleteToken = @"
            DELETE FROM refresh_tokens 
            WHERE token = @Token";

        private const string QuerySaveToken = @"
            INSERT INTO refresh_tokens (token, user_id, expiry_date) 
            VALUES (@Token, @UserId, @ExpiryDate)";

        private const string QueryGetToken = @"
            SELECT token 
            FROM refresh_tokens 
            WHERE user_id = @UserId 
            LIMIT 1";

        private const string QueryGetUser = @"
                SELECT u.Id, u.Name, u.Email FROM Users u INNER JOIN RefreshTokens rt ON rt.UserId = u.Id WHERE rt.Token = @Token AND rt.ExpiryDate > @Now";

        public RefreshTokenDao(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<UserModel> GetUserByTokenAsync(Guid refreshToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var user = await connection.QuerySingleOrDefaultAsync<UserModel>(
                    QueryGetUser,
                    new { Token = refreshToken, Now = DateTime.UtcNow }
                );

                return user;
            }
        }

        public async Task<bool> VerifyTokenAsync(Guid token)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                var result = await connection.ExecuteScalarAsync<int>(
                    QueryVerifyToken,
                    new { Token = token, Now = DateTime.UtcNow }
                );

                return result == 1; 
            }
        }

        public async Task<int> DeleteRefreshTokenAsync(Guid token)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                return await connection.ExecuteAsync(
                    QueryDeleteToken,
                    new { Token = token }
                );
            }
        }

        public async Task<int> SaveRefreshTokenAsync(Guid token, int userId, DateTime expiryDate)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                return await connection.ExecuteAsync(
                    QuerySaveToken,
                    new { Token = token, UserId = userId, ExpiryDate = expiryDate }
                );
            }
        }

        public async Task<Guid?> GetRefreshTokenAsync(int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                return await connection.ExecuteScalarAsync<Guid?>(
                    QueryGetToken,
                    new { UserId = userId }
                );
            }
        }
    }
}
