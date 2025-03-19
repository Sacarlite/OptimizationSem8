using System.Security.Cryptography;
using System.Text;

namespace OptimizationSem8.Service
{
    public static class GenerationService
    {
        /// <summary>
        /// Генерирует случайный пароль заданной длины.
        /// </summary>
        /// <param name="length">Длина пароля (по умолчанию 8 символов).</param>
        /// <returns>Случайный пароль.</returns>
        public static string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var result = new StringBuilder(length);
            foreach (var b in randomBytes)
            {
                result.Append(validChars[b % validChars.Length]);
            }

            return result.ToString();
        }
    }
}
