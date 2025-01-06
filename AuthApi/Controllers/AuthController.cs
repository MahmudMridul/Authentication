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
                RefreshToken? refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == storedToken);

                // there is a refresh token in the cookie
                if (!string.IsNullOrEmpty(storedToken))
                {
                    if (TokenService.RefreshTokenBelongsToUser(user, refreshToken))
                    {
                        if (!TokenService.UserHasValidRefreshToken(user, refreshToken))
                        {
                            // refresh token has expired. generate a new one
                            await TokenService.AddNewRefreshTokenForUser(user, Response, _context);
                        }
                    }
                    else
                    {
                        // this refresh token doesn't belong to current user. so overwrite the token
                        Response.Cookies.Delete("RefreshToken");
                        RefreshToken? refreshTokenForThisUser = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == user.Id);

                        if (TokenService.UserHasValidRefreshToken(user, refreshTokenForThisUser))
                        {
                            TokenService.SetRefreshTokenToCookies(refreshTokenForThisUser.Token, Response);
                        }
                        else
                        {
                            await TokenService.AddNewRefreshTokenForUser(user, Response, _context);
                        }
                    }
                }
                else
                {
                    // there is no refresh token set in the cookie
                    RefreshToken? refreshTokenForThisUser = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == user.Id);

                    if (TokenService.UserHasValidRefreshToken(user, refreshTokenForThisUser))
                    {
                        TokenService.SetRefreshTokenToCookies(refreshTokenForThisUser.Token, Response);
                    }
                    else
                    {
                        await TokenService.AddNewRefreshTokenForUser(user, Response, _context);
                    }
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
