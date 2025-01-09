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
    WHERE token = @Token AND expiry_date > @Now 
    LIMIT 1";

        private const string QueryDeleteToken = @"
    DELETE FROM T_REFRESH_TOKENS 
    WHERE token = @Token";

        private const string QuerySaveToken = @"
    INSERT INTO T_REFRESH_TOKENS (token, id_user, expiry_date) 
    VALUES (@Token, @id_user, @ExpiryDate)";

        private const string QueryGetToken = @"
    SELECT token 
    FROM T_REFRESH_TOKENS 
    WHERE id_user = @id_user 
    LIMIT 1";

        private const string QueryGetUser = @"
    SELECT u.id_user,
           u.fullname,
           u.email
    FROM T_USERS u 
    INNER JOIN T_REFRESH_TOKENS rt 
        ON rt.id_user = u.id_user
    WHERE rt.token = @Token AND rt.expiry_date > @Now";


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
                    new { Token = refreshToken.ToString(), Now = DateTime.UtcNow }
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
                    new { Token = token.ToString(), Now = DateTime.UtcNow }
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
                    new { Token = token.ToString(), id_user = userId, ExpiryDate = expiryDate }
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
                    new { id_User = userId }
                );
            }
        }
    }
}
