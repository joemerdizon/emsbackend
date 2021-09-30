using System;
using System.Security.Cryptography;

namespace EMS_Backend.Helper
{
    public class RefreshTokenGenerator
    {
        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateRefreshToken(int size = 32)
        {
            var refToken = new byte[size];
            using (var rng = RandomNumberGenerator.Create()) 
            {
                rng.GetBytes(refToken);

                return Convert.ToBase64String(refToken);
            }
        }
    }
}
