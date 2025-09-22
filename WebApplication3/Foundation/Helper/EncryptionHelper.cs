using System.Security.Cryptography;
using System.Text;

namespace WebApplication3.Foundation.Helper
{
    public static class EncryptionHelper
    {        
        // SHA256 加密
        public static string ComputeSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        // MD5 加密
        public static string ComputeMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        // Base64 编码
        public static string EncodeBase64(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes);
        }

        // Base64 解码
        public static string DecodeBase64(string base64Input)
        {
            byte[] decodedBytes = Convert.FromBase64String(base64Input);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}
