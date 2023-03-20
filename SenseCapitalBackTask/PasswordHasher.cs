using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace SenseCapitalBackTask
{
    public class PasswordHasher
    {
        /// <summary>
        /// Хэширование паролей
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <param name="saltString">Соль</param>
        /// <returns>Хэшированный пароль</returns>
        public static string Hash(string password, string saltString)
        {
            byte[] salt = Encoding.ASCII.GetBytes(saltString);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}