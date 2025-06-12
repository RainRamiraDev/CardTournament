namespace CTDao.Dao.Security
{
    using BCrypt.Net;
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            string hashedPassword = BCrypt.HashPassword(password);
            return hashedPassword;
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Verify(enteredPassword, storedHash);
        }
    }
}
