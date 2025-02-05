using CTDao.Interfaces.RefreshToken;
using CTDataModels.Users;
using Dapper;
using DataAccess;
using MySql.Data.MySqlClient;


namespace CTDao.Dao.RefreshToken
{
    public class RefreshTokenDao : IRefreshTokenDao
    {
        private readonly string _connectionString;

        public RefreshTokenDao(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<UserModel> GetUserByTokenAsync(Guid refreshToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var user = await connection.QuerySingleOrDefaultAsync<UserModel>(
                    QueryLoader.GetQuery("QueryGetUser"),
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
                    QueryLoader.GetQuery("QueryVerifyToken"),
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
                    QueryLoader.GetQuery("QueryDeleteToken"),
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
                    QueryLoader.GetQuery("QuerySaveToken"),
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
                    QueryLoader.GetQuery("QueryGetToken"),
                    new { id_User = userId }
                );
            }
        }
    }
}
