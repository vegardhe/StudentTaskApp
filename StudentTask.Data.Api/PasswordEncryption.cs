using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace StudentTask.Data.Api
{
    /// <summary>
    /// Encryption and verification of password hashes.
    /// </summary>
    public static class PasswordEncryption
    {
        /// <summary>
        /// Encrypts the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static string Encrypt(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(20);

            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="savedPasswordHash">The saved password hash.</param>
        /// <returns></returns>
        public static bool Verify(string password, string savedPasswordHash)
        {
            var hashBytes = Convert.FromBase64String(savedPasswordHash);

            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(20);
            for (var i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    }
}