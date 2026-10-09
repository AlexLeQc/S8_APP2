using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Cryptography;
using Sanssoussi.Areas.Identity.Data;

namespace Sanssoussi.Security
{
    public class CustomPasswordHasher : IPasswordHasher<SanssoussiUser>
    {
        private const int Iterations = 210000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public string HashPassword(SanssoussiUser user, string password)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            var keyBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithmName.SHA512, KeySize);
            
            var key = Convert.ToBase64String(keyBytes);
            var salt = Convert.ToBase64String(saltBytes);

            return $"{Iterations}.{salt}.{key}";
        }

        public PasswordVerificationResult VerifyHashedPassword(SanssoussiUser user, string hashedPassword, string providedPassword)
        {
            var parts = hashedPassword.Split('.', 3);
            if (parts.Length != 3)
            {
                var defaultHasher = new PasswordHasher<SanssoussiUser>();
                return defaultHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            }

            var iterations = Convert.ToInt32(parts[0]);
            var saltBytes = Convert.FromBase64String(parts[1]);
            var key = parts[2];

            var keyToCheckBytes = Rfc2898DeriveBytes.Pbkdf2(providedPassword, saltBytes, iterations, HashAlgorithmName.SHA512, KeySize);
            var keyToCheck = Convert.ToBase64String(keyToCheckBytes);

            if (keyToCheck == key)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}
