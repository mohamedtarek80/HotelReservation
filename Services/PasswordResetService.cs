using System.Security.Cryptography;

namespace HotelReservation.Services
{
    public class PasswordResetService
    {
        public string GenerateToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
    }
}
