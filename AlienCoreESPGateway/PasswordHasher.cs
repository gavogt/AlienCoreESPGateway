using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace AlienCoreESPGateway
{
    public class PasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string plaintextPassword)
        {
            if (string.IsNullOrEmpty(plaintextPassword))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(plaintextPassword));
            }
            else
            {

                return _hasher.HashPassword(null!, plaintextPassword);
            }

            
        }

        public bool Verify(string hashedPassword, string plaintextPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, plaintextPassword);
            return result != PasswordVerificationResult.Failed;

        }
    }
}
