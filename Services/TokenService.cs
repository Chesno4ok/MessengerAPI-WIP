using System.Security.Cryptography;
using System.Text;

namespace ChesnokMessengerAPI.Services
{
    public class TokenService
    {
        public static string GenerateToken() => Guid.NewGuid().ToString();
        public static string GenerateHash(Guid token)
        {
            MD5 MD5Hash = MD5.Create();

            byte[] byteToken = MD5Hash.ComputeHash(token.ToByteArray());

            return GetHashString(byteToken);
        }
        public static string GetHashString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
