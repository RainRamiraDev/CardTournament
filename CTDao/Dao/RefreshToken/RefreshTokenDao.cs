using CTDao.Interfaces.RefreshToken;
using CTDataModels.Users;
using Dapper;
using MySql.Data.MySqlClient;


namespace CTDao.Dao.RefreshToken
{
    public class RefreshTokenDao : IRefreshTokenDao
    {
        private readonly string _connectionString;

        private const string QueryVerifyToken = @"
            SELECT 1
            FROM T_REFRESH_TOKENS 
            WHERE id_token = @Token AND expiry_date > @Now 
            LIMIT 1";

        private const string QueryDeleteToken = @"
            DELETE FROM T_REFRESH_TOKENS 
            WHERE id_token = @Token";

        private const string QuerySaveToken = @"
            INSERT INTO T_REFRESH_TOKENS (id_token, id_user, expiry_date) 
            VALUES (@Token, @UserId, @ExpiryDate)";

        private const string QueryGetToken = @"
            SELECT token 
            FROM T_REFRESH_TOKENS 
            WHERE Id_user = @UserId 
            LIMIT 1";

        private const string QueryGetUser = @"
                SELECT u.Id_user,
 		               u.Fullname,
  		               u.Email FROM T_Users u INNER JOIN T_Refresh_Tokens rt ON rt.id_user = u.id_user
                WHERE rt.Token = @Token AND rt.ExpiryDate > @NOW";

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
