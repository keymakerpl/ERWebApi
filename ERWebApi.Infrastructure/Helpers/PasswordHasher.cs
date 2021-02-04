using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace ERService.Infrastructure.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        public void GenerateSaltedHash(string password, out string hash, out string salt)
        {
            var saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);
            hash = GetHash(password, saltBytes);
        }

        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var hash = GetHash(enteredPassword, saltBytes);

            return hash == storedHash;
        }

        private static string GetHash(string password, byte[] saltBytes)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                ));
        }
    }    
}
