namespace CTDao.Dao.Security
{
    using BCrypt.Net;
    public class PasswordHasher
    {
        public string HashPassword(string password)
        {
            string hashedPassword = BCrypt.HashPassword(password);
            Console.WriteLine(hashedPassword);
            return hashedPassword;
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {

            Console.WriteLine(enteredPassword);
            return BCrypt.Verify(enteredPassword, storedHash);

        }
    }
}
