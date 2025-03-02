using CTConfigurations;
using CTDao.Interfaces.RefreshToken;
using CTService.Interfaces.RefreshToken;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Implementation.RefreshToken
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenDao _refreshTokenDao;
        private readonly KeysConfiguration _keysConfiguration;


        public RefreshTokenService(IRefreshTokenDao refreshTokenDao, IOptions<KeysConfiguration> keysConfiguration)
        {
            _refreshTokenDao = refreshTokenDao ?? throw new ArgumentNullException(nameof(refreshTokenDao));
            _keysConfiguration = keysConfiguration.Value; 
          
        }

        private (Guid Token, DateTime Expiration) GenerateRefreshToken()
        {
            Guid newRefreshToken = Guid.NewGuid();
            DateTime expirationDate = DateTime.UtcNow.AddDays(_keysConfiguration.ExpirationDays);

            return (newRefreshToken, expirationDate);
        }

        public async Task<(string AccessToken, Guid RefreshToken)> RefreshAccessTokenAsync(Guid oldRefreshToken)
        {

            bool isValidToken = await _refreshTokenDao.VerifyTokenAsync(oldRefreshToken);
            if (!isValidToken)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = await _refreshTokenDao.GetUserByTokenAsync(oldRefreshToken);


            if (user == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");
 

            await _refreshTokenDao.DeleteRefreshTokenAsync(oldRefreshToken);

            string newAccessToken = await GenerateAccessTokenAsync(user.Id_Rol, user.Fullname,user.Id_Rol);

            var (newRefreshToken, expirationDate) = GenerateRefreshToken();

            await _refreshTokenDao.SaveRefreshTokenAsync(newRefreshToken, user.Id_User, expirationDate);

            return (newAccessToken, newRefreshToken);
        }

        public async Task<bool> LogoutAsync(Guid refreshToken)
        {
            int rowsAffected = await _refreshTokenDao.DeleteRefreshTokenAsync(refreshToken);
            return rowsAffected > 0;
        }

        public async Task<string> GenerateAccessTokenAsync(int userId, string userName, int userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_keysConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
            {
                new Claim("UserName", userName),
                new Claim("UserId", userId.ToString()), 
                new Claim("UserRole", userRole.ToString()) 
            };

            var token = new JwtSecurityToken(
                issuer: _keysConfiguration.Issuer,
                audience: _keysConfiguration.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_keysConfiguration.ExpirationDays),
                signingCredentials: credentials
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task SaveRefreshTokenAsync(Guid token, int userId, DateTime expiryDate)
        {
            await _refreshTokenDao.SaveRefreshTokenAsync(token, userId, expiryDate);
        }
    }
}

