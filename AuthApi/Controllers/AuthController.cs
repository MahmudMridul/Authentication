using AuthApi.Db;
using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(AuthContext context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse>> Signup([FromBody] SignupDto model)
        {
            ApiResponse res;
            try
            {
                if (!ModelState.IsValid)
                {
                    List<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    res = ApiResponse.Create(HttpStatusCode.BadRequest, msg: "Validation failed", errors: errors);
                    return BadRequest(res);
                }

                var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName || u.Email == model.Email);

                if (existingUser != null)
                {
                    string conflictField = existingUser.UserName == model.UserName ? "Username" : "Email";
                    res = ApiResponse.Create(HttpStatusCode.Conflict, msg: $"{conflictField} already exists");
                    return Conflict(res);
                }

                User user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    AccessFailedCount = 0,
                    LockoutEnabled = true,
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    List<string> errorMsg = result.Errors.Select(e => e.Description).ToList();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Signup failed");
                    foreach (string err in errorMsg)
                    {
                        sb.AppendLine(err);
                    }
                    res = ApiResponse.Create(HttpStatusCode.BadRequest, msg: sb.ToString());
                    return BadRequest(res);
                }

                await _userManager.AddToRoleAsync(user, "User");
                res = ApiResponse.Create(HttpStatusCode.OK, model, true, "Signup successfull", null);
                return Ok(res);
            }
            catch (Exception e)
            {
                List<string> errs = new List<string> { e.Message, e.StackTrace };
                res = ApiResponse.Create(HttpStatusCode.InternalServerError, msg: "An unexpected error occurred", errors: errs);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }

        [HttpPost("signin")]
        public async Task<ActionResult<ApiResponse>> Signin([FromBody] SigninDto model)
        {
            ApiResponse res;
            try
            {
                if (!ModelState.IsValid)
                {
                    List<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    res = ApiResponse.Create(HttpStatusCode.BadRequest, msg: "Validation failed", errors: errors);
                    return BadRequest(res);
                }

                User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.NameOrEmail || u.Email == model.NameOrEmail);
                if (user == null)
                {
                    res = ApiResponse.Create(HttpStatusCode.NotFound, msg: "No user found");
                    return NotFound(res);
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    DateTimeOffset? lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    TimeSpan remainingTime = lockoutEnd.HasValue ?
                        lockoutEnd.Value.Subtract(DateTimeOffset.UtcNow) :
                        TimeSpan.Zero;

                    res = ApiResponse.Create(HttpStatusCode.Unauthorized,
                        msg: $"Account is locked. Try again in {remainingTime.Minutes} minutes.");
                    return Unauthorized(res);
                }

                SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);

                if (!result.Succeeded)
                {
                    int maxAttempts = 5;
                    int failedCount = await _userManager.GetAccessFailedCountAsync(user);
                    int attemptsLeft = maxAttempts - failedCount;

                    if (result.IsLockedOut)
                    {
                        res = ApiResponse.Create(HttpStatusCode.Unauthorized,
                            msg: "Account has been locked due to too many failed attempts. Try again in 5 minutes.");
                    }
                    else
                    {
                        res = ApiResponse.Create(HttpStatusCode.Unauthorized,
                            msg: $"Invalid username or password. {attemptsLeft} attempts remaining before lockout.");
                    }
                    return Unauthorized(res);
                }

                string accessToken = await TokenService.GenerateJwtToken(user, _userManager, _config);
                string? storedToken = Request.Cookies["RefreshToken"];
                RefreshToken? refreshToken = await _context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == storedToken);

                // there is a refresh token in the cookie
                if (!string.IsNullOrEmpty(storedToken))
                {
                    if (RefreshTokenBelongsToUser(user, refreshToken))
                    {
                        if (refreshToken.Expires < DateTime.UtcNow)
                        {
                            // refresh token has expired. generate a new one
                            (string token, DateTime expiration) = TokenService.GenerateRefreshToken();
                            refreshToken.Token = token;
                            refreshToken.Expires = expiration;
                            refreshToken.CreatedOn = DateTime.UtcNow;
                            refreshToken.RevokedOn = null;
                            _context.RefreshTokens.Update(refreshToken);
                            await _context.SaveChangesAsync();
                            var refreshTokenOps = TokenService.GetCookieOptions(7, false);
                            Response.Cookies.Append("RefreshToken", refreshToken.Token, refreshTokenOps);
                        }
                        else
                        {
                            // refresh token is still valid. so do nothing
                            var refreshTokenOps = TokenService.GetCookieOptions(7, false);
                            Response.Cookies.Append("RefreshToken", refreshToken.Token, refreshTokenOps);
                        }
                    }
                    else
                    {
                        // this refresh token doesn't belong to current user. so overwrite the token
                        Response.Cookies.Delete("RefreshToken");
                    }
                }
                else if (UserHasValidRefreshToken(user, refreshToken))
                {
                    SetRefreshTokenToCookies(refreshToken.Token);
                }
                else
                {
                    // user has valid no refresh token
                    await AddNewRefreshTokenForUser(user);
                }

                res = ApiResponse.Create(
                    HttpStatusCode.OK,
                    new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        accessToken = accessToken,
                    },
                    true,
                    "Sign in successfull"
                );
                return Ok(res);

            }
            catch (Exception e)
            {
                List<string> errs = new List<string> { e.Message, e.StackTrace };
                res = ApiResponse.Create(HttpStatusCode.InternalServerError, msg: "An unexpected error occurred", errors: errs);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }

        private bool RefreshTokenBelongsToUser(User user, RefreshToken? refreshToken)
        {
            return refreshToken != null && refreshToken.UserId == user.Id;
        }

        private bool UserHasValidRefreshToken(User user, RefreshToken? refreshToken)
        {
            return refreshToken != null && refreshToken.UserId == user.Id && refreshToken.Expires > DateTime.UtcNow;
        }

        private void SetRefreshTokenToCookies(string token)
        {
            var refreshTokenOps = TokenService.GetCookieOptions(7, false);
            Response.Cookies.Append("RefreshToken", token, refreshTokenOps);
        }

        private async Task<string> AddNewRefreshTokenForUser(User user)
        {
            (string token, DateTime expiration) = TokenService.GenerateRefreshToken();
            RefreshToken newRefreshToken = new RefreshToken
            {
                Token = token,
                Expires = expiration,
                CreatedOn = DateTime.UtcNow,
                RevokedOn = null,
                UserId = user.Id
            };
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();
            SetRefreshTokenToCookies(newRefreshToken.Token);
            return newRefreshToken.Token;
        }

        [HttpGet("is-authenticated")]
        public async Task<ActionResult<ApiResponse>> IsAuthenticated()
        {
            ApiResponse res;
            try
            {
                string? accessToken = Request.Cookies["AccessToken"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    res = ApiResponse.Create(
                        HttpStatusCode.Unauthorized,
                        msg: "Access token not found"
                    );
                    return Unauthorized(res);
                }

                res = ApiResponse.Create(
                    HttpStatusCode.OK,
                    success: true,
                    msg: "User is authenticated"
                );
                return Ok(res);
            }
            catch (Exception e)
            {
                List<string> errs = new List<string> { e.Message, e.StackTrace };
                res = ApiResponse.Create(HttpStatusCode.InternalServerError, msg: "An unexpected error occurred", errors: errs);
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }

        [HttpPost("signout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> Signout()
        {
            ApiResponse res;
            try
            {
                User? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    res = ApiResponse.Create(
                        HttpStatusCode.Unauthorized,
                        msg: "User not found"
                    );
                    return Unauthorized(res);
                }

                //user.RefreshToken = "";
                //user.RefreshTokenExpiryTime = DateTime.MinValue;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await _signInManager.SignOutAsync();

                res = ApiResponse.Create(
                    HttpStatusCode.OK,
                    success: true,
                    msg: "Sign out successful"
                );
                return Ok(res);
            }
            catch (Exception ex)
            {
                res = ApiResponse.Create(
                    statusCode: HttpStatusCode.InternalServerError,
                    msg: "An error occurred during sign out",
                    errors: new List<string> { ex.Message, ex.StackTrace }
                );
                return StatusCode((int)HttpStatusCode.InternalServerError, res);
            }
        }
    }
}
