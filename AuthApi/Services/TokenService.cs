using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthApi.Services
{
    public class TokenService
    {
        public static async Task<string> GenerateJwtToken(User user, UserManager<User> userManager, IConfiguration config)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles to claims
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static (string token, DateTime expiration) GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var token = Convert.ToBase64String(randomNumber);
                var expiration = DateTime.UtcNow.Add(TimeSpan.FromDays(7));
                return (token, expiration);
            }
        }

        public static CookieOptions GetCookieOptions(int timeSpan, bool isHours = true)
        {
            return new CookieOptions
            {
                HttpOnly = true,       // Prevents access via JavaScript
                Secure = true,         // Ensures the cookie is only sent over HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = isHours ? DateTime.UtcNow.AddHours(timeSpan) : DateTime.UtcNow.AddDays(timeSpan)
            };
        }
    }
}
