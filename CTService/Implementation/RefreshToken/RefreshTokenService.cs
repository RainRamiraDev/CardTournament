﻿using Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserDaoLib.Daos.Interfaces;

namespace RefreshTokenApp.Service.Interface
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

        public async Task<(string AccessToken, Guid RefreshToken)> RefreshAccessTokenAsync(Guid oldRefreshToken)
        {
            bool isValidToken = await _refreshTokenDao.VerifyTokenAsync(oldRefreshToken);
            if (!isValidToken)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = await _refreshTokenDao.GetUserByTokenAsync(oldRefreshToken); 
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            await _refreshTokenDao.DeleteRefreshTokenAsync(oldRefreshToken);

       
            string newAccessToken = await GenerateAccessToken(user.Id, user.Nombre, user.Email);  

     
            Guid newRefreshToken = Guid.NewGuid();
            DateTime expirationDate = DateTime.UtcNow.AddDays(7);

  
            await _refreshTokenDao.SaveRefreshTokenAsync(newRefreshToken, user.Id, expirationDate);

            return (newAccessToken, newRefreshToken);
        }


        public async Task<bool> LogoutAsync(Guid refreshToken)
        {
            int rowsAffected = await _refreshTokenDao.DeleteRefreshTokenAsync(refreshToken);
            return rowsAffected > 0;
        }

        public async Task<string> GenerateAccessToken(int userId, string userName, string userEmail)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_keysConfiguration.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "usuario")  
            };

            var token = new JwtSecurityToken(
                issuer: _keysConfiguration.Issuer,
                audience: _keysConfiguration.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
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

