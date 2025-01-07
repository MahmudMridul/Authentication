using AuthApi.Db;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
                expires: DateTime.UtcNow.AddMinutes(30),
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
                SameSite = SameSiteMode.None,
                Expires = isHours ? DateTime.UtcNow.AddHours(timeSpan) : DateTime.UtcNow.AddDays(timeSpan)
            };
        }

        public static bool RefreshTokenBelongsToUser(string storedToken, RefreshToken? refreshToken)
        {
            return refreshToken != null && storedToken != null && refreshToken.Token == storedToken;
        }

        public static bool UserHasValidRefreshToken(RefreshToken? refreshToken)
        {
            return refreshToken != null && refreshToken.Expires > DateTime.UtcNow;
        }

        public static void SetRefreshTokenToCookies(string token, HttpResponse response)
        {
            var refreshTokenOps = GetCookieOptions(7, false);
            response.Cookies.Append("RefreshToken", token, refreshTokenOps);
        }

        public static async Task<string> AddNewRefreshTokenForUser(string userId, HttpResponse response, AuthContext _context)
        {
            (string token, DateTime expiration) = GenerateRefreshToken();
            RefreshToken newRefreshToken = new RefreshToken
            {
                Token = token,
                Expires = expiration,
                CreatedOn = DateTime.UtcNow,
                RevokedOn = null,
                UserId = userId
            };
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();
            SetRefreshTokenToCookies(newRefreshToken.Token, response);
            return newRefreshToken.Token;
        }

        public static async Task<bool> HandleNoStoredToken(string userId, RefreshToken? refreshToken, AuthContext _context, HttpResponse response)
        {
            try
            {
                if (UserHasValidRefreshToken(refreshToken))
                {
                    SetRefreshTokenToCookies(refreshToken.Token, response);
                    return true;
                }
                else
                {
                    await 
                        
                        AddNewRefreshTokenForUser(userId, response, _context);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
