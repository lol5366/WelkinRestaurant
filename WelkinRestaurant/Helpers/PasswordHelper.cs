using System;
using System.Security.Cryptography;
using System.Text;

namespace WelkinRestaurant.Helpers
{
    public static class PasswordHelper
    {
        // Генерация случайной соли
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Хэширование пароля с солью
        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // Проверка пароля
        public static bool VerifyPassword(string enteredPassword, string storedHash, string salt)
        {
            string hashOfEntered = HashPassword(enteredPassword, salt);
            return hashOfEntered == storedHash;
        }
    }
}