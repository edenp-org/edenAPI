using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using WebApplication3.Foundation.Helper;

namespace WebApplication3.Sundry
{
    public static class TokenService
    {
        private static readonly string _secretKey = ConfigHelper.GetString("TokenKey");
        private static readonly string _issuer = ConfigHelper.GetString("TokenIssuer");
        private static readonly string _audience = ConfigHelper.GetString("TokenAudience");

        /// <summary>
        /// 生成JWT Token
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="role">用户角色</param>
        /// <param name="purpose">Token的作用</param>
        /// <returns>生成的Token字符串</returns>
        public static string GenerateToken(string username, string role, string purpose,long UCode, DateTime expires)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("Purpose", purpose),
                    new Claim("UCode", UCode.ToString())
                };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 验证JWT Token
        /// </summary>
        /// <param name="token">Token字符串</param>
        /// <returns>验证通过的ClaimsPrincipal对象，验证失败返回null</returns>
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrEmpty(_issuer),
                ValidIssuer = _issuer,
                ValidateAudience = !string.IsNullOrEmpty(_audience),
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
