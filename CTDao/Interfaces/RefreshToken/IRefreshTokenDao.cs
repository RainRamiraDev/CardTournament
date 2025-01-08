
using CTDataModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CTDao.Interfaces.RefreshToken

{
    public interface IRefreshTokenDao
    {
        Task<bool> VerifyTokenAsync(Guid token);
        Task<int> DeleteRefreshTokenAsync(Guid token);
        Task<int> SaveRefreshTokenAsync(Guid token, int userId, DateTime expiryDate);
        Task<Guid?> GetRefreshTokenAsync(int userId);
        Task<UserModel> GetUserByTokenAsync(Guid refreshToken);
    }
}
